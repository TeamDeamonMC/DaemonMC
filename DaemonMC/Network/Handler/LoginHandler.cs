using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using DaemonMC.Utils.Text;
using Newtonsoft.Json;

namespace DaemonMC.Network.Handler
{
    public class LoginHandler
    {
        public static void execute(Login packet, IPEndPoint clientEp)
        {
            byte[] jwtBuffer = Encoding.UTF8.GetBytes(packet.Request);
            string filteredJWT;
            string tokenJWT;

            if (packet.ProtocolVersion >= Info.v1_21_90)
            {
                int jsonStart = packet.Request.IndexOf('{');
                int jsonEnd = packet.Request.LastIndexOf("}");

                string jsonPart = packet.Request.Substring(jsonStart, jsonEnd - jsonStart + 1);

                var loginJson = JsonConvert.DeserializeObject<LoginJson>(jsonPart);
                var certJson = JsonDocument.Parse(loginJson.Certificate);
                var chainArray = certJson.RootElement.GetProperty("chain");

                filteredJWT = $"{{\"chain\":{chainArray.GetRawText()}}}";

                byte[] jwtBytes = Encoding.UTF8.GetBytes(packet.Request);
                int tokenStart = Encoding.UTF8.GetBytes(jsonPart).Length + 8;

                tokenJWT = Encoding.UTF8.GetString(jwtBytes, tokenStart, jwtBytes.Length - tokenStart);
            }
            else
            {
                string pattern = @"{""chain"":\[.*?\]}";
                var match = Regex.Match(packet.Request, pattern);
                filteredJWT = match.Value;

                int tokenStart = Encoding.UTF8.GetBytes(filteredJWT).Length + 8;
                tokenJWT = Encoding.UTF8.GetString(jwtBuffer, tokenStart, jwtBuffer.Length - tokenStart);
            }

            JWT.processJWTchain(filteredJWT, clientEp);
            JWT.processJWTtoken(tokenJWT, clientEp);

            var player = RakSessionManager.getSession(clientEp);

            if (Cryptography.UseEncryption && player.XUID != "")
            {
                byte[] publicKeyBytes = Convert.FromBase64String(player.identityPublicKey);

                Cryptography.verifyKeyInfo(publicKeyBytes, player.identityPublicKey);

                using var ecdhRemote = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
                ecdhRemote.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

                using var ecdhLocal = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
                var localParams = ecdhLocal.ExportParameters(true);

                byte[] sharedSecret = ecdhLocal.DeriveKeyMaterial(ecdhRemote.PublicKey);

                Cryptography.verifySharedSecret(sharedSecret);

                byte[] secretPrepend = new byte[16];
                RandomNumberGenerator.Fill(secretPrepend);
                Log.debug($"SecretPrepend: {Convert.ToBase64String(secretPrepend)}");
                byte[] combined = secretPrepend.Concat(sharedSecret).ToArray();
                byte[] aesKey = SHA256.Create().ComputeHash(combined);

                Cryptography.verifyAESKey(aesKey);

                player.encryptor = new Encryptor(aesKey, aesKey);

                var ecdsaParam = new ECParameters
                {
                    Curve = ECCurve.NamedCurves.nistP384,
                    D = localParams.D,
                    Q = new ECPoint
                    {
                        X = localParams.Q.X,
                        Y = localParams.Q.Y
                    }
                };

                ecdsaParam.Validate();

                using var ecdsa = ECDsa.Create(ecdsaParam);

                string chain = JWT.CreateHandshakeJwt(secretPrepend, ecdsa);

                PacketEncoder encoder1 = PacketEncoderPool.Get(clientEp);
                var pk1 = new ServerToClientHandshake
                {
                    JWT = chain,
                };
                pk1.EncodePacket(encoder1);
            }
            else
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new PlayStatus
                {
                    Status = 0,
                };
                pk.EncodePacket(encoder);
            }
        }
    }

    public class LoginJson
    {
        public int AuthenticationType { get; set; } = 0;
        public string Certificate { get; set; } = "";
    }
}

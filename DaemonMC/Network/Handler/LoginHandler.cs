using System.Net;
using System.Security.Cryptography;
using System.Text;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using Newtonsoft.Json;

namespace DaemonMC.Network.Handler
{
    public class LoginHandler
    {
        public static bool XboxAuth { get; set; } = true;

        public enum AuthenticationType
        {
            Full,
            Guest,
            SelfSigned
        }

        public static void handleRequest(Login packet, IPEndPoint clientEp)
        {
            byte[] Buffer = packet.Request;

            using (var ms = new MemoryStream(Buffer))
            using (var reader = new BinaryReader(ms))
            {
                string authToken;
                string tokenJWT;

                uint chainLength = reader.ReadUInt32();
                string jsonPart = Encoding.UTF8.GetString(reader.ReadBytes((int)chainLength));

                var loginJson = JsonConvert.DeserializeObject<LoginJson>(jsonPart);

                if (XboxAuth && loginJson.AuthenticationType != (int)AuthenticationType.Full)
                {
                    Reject(clientEp, "You need to login to Xbox Live");
                    return;
                }

                authToken = loginJson.Token;

                uint tokenLength = reader.ReadUInt32();
                tokenJWT = Encoding.UTF8.GetString(reader.ReadBytes((int)tokenLength));

                JWT.processAuthToken(authToken, clientEp);
                JWT.processJWTtoken(tokenJWT, clientEp);
            }

            var player = RakSessionManager.getSession(clientEp);

            if (Cryptography.UseEncryption && player.XUID != "")
            {
                byte[] publicKeyBytes = Convert.FromBase64String(player.identityPublicKey);

                Cryptography.verifyKeyInfo(publicKeyBytes, player.identityPublicKey);

                var ecdhRemote = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
                ecdhRemote.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

                var ecdhLocal = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
                var localParams = ecdhLocal.ExportParameters(true);

                byte[] sharedSecret = ecdhLocal.DeriveRawSecretAgreement(ecdhRemote.PublicKey);
                Cryptography.verifySharedSecret(sharedSecret);

                byte[] secretPrepend = new byte[16];
                RandomNumberGenerator.Fill(secretPrepend);

                byte[] aesKey = SHA256.HashData(secretPrepend.Concat(sharedSecret).ToArray());

                Cryptography.verifyAESKey(aesKey);

                player.encryptor = new Encryptor(aesKey);

                localParams.Validate();

                var ecdsa = ECDsa.Create(localParams);

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

        public static byte[] createRequest()
        {
            var loginJson = new LoginJson
            {
                AuthenticationType = (int)AuthenticationType.SelfSigned,
                Certificate = $"{{\"chain\":[\"{JWT.CreateJWTchain()}\"]}}"
            };

            string jsonPart = JsonConvert.SerializeObject(loginJson);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonPart);

            string tokenJWT = JWT.CreateJWTtoken();
            byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenJWT);

            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write((uint)jsonBytes.Length);

                writer.Write(jsonBytes);

                writer.Write((uint)tokenBytes.Length);

                writer.Write(tokenBytes);

                return ms.ToArray();
            }
        }

        public static void Reject(IPEndPoint clientEp, string reason)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var packet = new Disconnect
            {
                Message = reason
            };
            packet.EncodePacket(encoder);
        }
    }

    public class LoginJson
    {
        public int AuthenticationType { get; set; } = 0;
        public string Certificate { get; set; } = ""; //Deprecated
        public string Token { get; set; } = "";
    }
}

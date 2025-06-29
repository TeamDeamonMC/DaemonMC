﻿using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using DaemonMC.Utils.Text;
using Newtonsoft.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DaemonMC.Network.Handler
{
    public class LoginHandler
    {
        public static void execute(Login packet, IPEndPoint clientEp)
        {
            byte[] jwtBuffer = Encoding.UTF8.GetBytes(packet.Request);
            string filteredJWT;

            if (packet.ProtocolVersion >= Info.v1_21_90)
            {
                var request = packet.Request.Substring(packet.Request.IndexOf('{')).TrimStart().Split('\n')[0];

                var json = JsonConvert.DeserializeObject<LoginJson>(request);
                var certificateJsonString = json.Certificate;
                var certificateJson = JsonDocument.Parse(certificateJsonString);
                var chainArray = certificateJson.RootElement.GetProperty("chain");

                filteredJWT = $"{{\"chain\":{chainArray.GetRawText()}}}";
            }
            else
            {
                string pattern = @"{""chain"":\[.*?\]}";
                var match = Regex.Match(packet.Request, pattern);
                filteredJWT = match.Value;
            }

            int tokenStartIndex = Encoding.UTF8.GetBytes(filteredJWT).Length + 8;
            var Token = Encoding.UTF8.GetString(jwtBuffer, tokenStartIndex, jwtBuffer.Length - tokenStartIndex);

            JWT.processJWTchain(filteredJWT, clientEp);
            JWT.processJWTtoken(Token, clientEp);

            var player = RakSessionManager.getSession(clientEp);

            if (Cryptography.UseEncryption && player.XUID != "")
            {
                byte[] publicKeyBytes = Convert.FromBase64String(player.identityPublicKey);
                Cryptography.verifyKeyInfo(publicKeyBytes);

                Log.debug($"Client Public Key (Base64): {Convert.ToBase64String(publicKeyBytes)}");
                using var ecdhRemote = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
                ecdhRemote.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

                using var ecdhLocal = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP384);
                using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP384);

                byte[] localPublicKey = ecdhLocal.PublicKey.ToByteArray();

                byte[] sharedSecret = ecdhLocal.DeriveKeyMaterial(ecdhRemote.PublicKey);

                byte[] secretPrepend = Encoding.UTF8.GetBytes("");
                using var sha256 = SHA256.Create();
                byte[] finalSecret = sha256.ComputeHash(secretPrepend.Concat(sharedSecret).ToArray());
                Log.debug($"AES Key: {BitConverter.ToString(finalSecret)}");

                var valid = finalSecret.Length == 32 ? "OK" : "INVALID";
                Log.debug($"SHA256: {valid} {Convert.ToBase64String(finalSecret)}");

                if (valid == "OK")
                {
                    player.encryptor = new Encryptor(finalSecret);
                }

                Log.debug($"Public Key (Base64): {Convert.ToBase64String(publicKeyBytes)}");

                Log.debug($"Salt: {Convert.ToBase64String(secretPrepend)}");

                string chain = JWT.CreateHandshakeJwt(secretPrepend, ecdsa);

                PacketEncoder encoder1 = PacketEncoderPool.Get(clientEp);
                var pk1 = new ServerToClientHandshake
                {
                    JWT = chain,
                };
                pk1.EncodePacket(encoder1);
               // Log.debug(pk1.JWT);
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

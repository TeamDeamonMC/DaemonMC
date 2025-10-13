using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DaemonMC.Utils
{
    public class JWTObject
    {
        public List<string> Chain { get; set; } = new List<string>();
    }

    public class JWT
    {
        public static void processAuthToken(string authToken, IPEndPoint clientEp)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(authToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                LoginHandler.Reject(clientEp, "Invalid auth token");
                return;
            }

            var payload = jsonToken.Payload;

            var player = RakSessionManager.getSession(clientEp);
            player.XUID = payload.Claims.FirstOrDefault(claim => claim.Type == "xid").Value;

            if (player.XUID == null)
            {
                LoginHandler.Reject(clientEp, "Invalid auth token");
                return;
            }

            player.username = payload.Claims.FirstOrDefault(claim => claim.Type == "xname").Value;
            player.identityPublicKey = payload.Claims.FirstOrDefault(claim => claim.Type == "cpk").Value;

            byte[] data;
            using (var md5 = MD5.Create())
            {
                data = md5.ComputeHash(Encoding.UTF8.GetBytes("pocket-auth-1-xuid:" + player.XUID));
            }

            data[6] = (byte)((data[6] & 0x0F) | 0x30);

            data[8] = (byte)((data[8] & 0x3F) | 0x80);

            Array.Reverse(data, 0, 4);
            Array.Reverse(data, 4, 2);
            Array.Reverse(data, 6, 2);

            player.identity = new Guid(data).ToString();
        }

        public static void processJWTtoken(string rawToken, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            int index = rawToken.IndexOf("ey");
            string[] tokenParts = rawToken.Substring(index).Split('.');

            string headerJson = Encoding.UTF8.GetString(DecodeBase64Url(tokenParts[0]));
            string payloadJson = Encoding.UTF8.GetString(DecodeBase64Url(tokenParts[1]));

            JObject header = JObject.Parse(headerJson);
            JwtPayload payload = JsonConvert.DeserializeObject<JwtPayload>(payloadJson);

            try
            {
                player.skin = new Skin()
                {
                    ArmSize = payload.ArmSize,
                    AnimatedImageData = payload.AnimatedImageData,
                    OverrideSkin = payload.OverrideSkin,
                    PersonaPieces = payload.PersonaPieces,
                    PersonaSkin = payload.PersonaSkin,
                    PlayFabId = payload.PlayFabId,
                    PremiumSkin = payload.PremiumSkin,
                    SkinAnimationData = payload.SkinAnimationData,
                    SkinColor = payload.SkinColor,
                    PieceTintColors = payload.PieceTintColors,
                    SkinData = Convert.FromBase64String(payload.SkinData),
                    SkinGeometryData = Encoding.UTF8.GetString(Convert.FromBase64String(payload.SkinGeometryData)),
                    SkinGeometryDataEngineVersion = Encoding.UTF8.GetString(Convert.FromBase64String(payload.SkinGeometryDataEngineVersion)),
                    SkinId = payload.SkinId,
                    SkinImageHeight = payload.SkinImageHeight,
                    SkinImageWidth = payload.SkinImageWidth,
                    SkinResourcePatch = Encoding.UTF8.GetString(Convert.FromBase64String(payload.SkinResourcePatch)),
                    CapeOnClassicSkin = payload.CapeOnClassicSkin,
                    Cape = new Cape()
                    {
                        CapeData = Convert.FromBase64String(payload.CapeData),
                        CapeId = payload.CapeId,
                        CapeImageHeight = payload.CapeImageHeight,
                        CapeImageWidth = payload.CapeImageWidth,
                        CapeOnClassicSkin = payload.CapeOnClassicSkin
                    }
                };
            }
            catch (FormatException ex)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var packet = new Disconnect
                {
                    Message = $"Skin decoding failed"
                };
                packet.EncodePacket(encoder);
                Log.error($"Skin decoding failed: {ex.Message}");
            }

            Log.info($"{player.username} with client version {payload.GameVersion} doing login...");
        }

        public static string CreateJWTchain()
        {
            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP384);
            var keyParams = ecdsa.ExportParameters(true);
            var securityKey = new ECDsaSecurityKey(ecdsa);

            var extraData = new
            {
                XUID = "",
                displayName = "Daemon123",
                identity = "e0e8697d-d37d-3c5c-816b-474dc9418b58"
            };

            var now = DateTimeOffset.UtcNow;
            var payloadDict = new Dictionary<string, object>
        {
            { "exp", now.AddYears(1).ToUnixTimeSeconds() },
            { "nbf", now.ToUnixTimeSeconds() },
            { "identityPublicKey", ExportPublicKeyToX509Base64(keyParams) },
            { "extraData", extraData }
        };

            string json = JsonConvert.SerializeObject(payloadDict);
            var jwtPayload = System.IdentityModel.Tokens.Jwt.JwtPayload.Deserialize(json);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha384);
            var header = new JwtHeader(credentials);
            header["x5u"] = ExportPublicKeyToX509Base64(keyParams);

            var token = new JwtSecurityToken(header, jwtPayload);

            var handler = new JwtSecurityTokenHandler();
            string jwt = handler.WriteToken(token);

            return jwt;
        }

        public static string CreateJWTtoken()
        {
            using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP384);
            var securityKey = new ECDsaSecurityKey(ecdsa);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.EcdsaSha384);

            var data = new JwtPayload();
            data.GameVersion = "1.21.111";
            data.ClientRandomId = new Random().Next();
            data.DeviceId = Guid.NewGuid().ToString();
            data.DeviceOS = (int)BuildPlatform.UWP;
            data.LanguageCode = "en_US";

            string payloadJson = JsonConvert.SerializeObject(data);
            var jwtPayload = System.IdentityModel.Tokens.Jwt.JwtPayload.Deserialize(payloadJson);

            var header = new JwtHeader(credentials);
            header["typ"] = "JWT";

            var token = new JwtSecurityToken(header, jwtPayload);
            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(token);
        }

        private static string ExportPublicKeyToX509Base64(ECParameters keyParams)
        {
            using var ec = ECDsa.Create(keyParams);
            var pubKey = ec.ExportSubjectPublicKeyInfo();
            return Convert.ToBase64String(pubKey);
        }

        public static string CreateHandshakeJwt(byte[] secret, ECDsa ecdsa)
        {
            byte[] spki = ecdsa.ExportSubjectPublicKeyInfo();
            string b64Spkiy = Convert.ToBase64String(spki);

            var headerJson = $"{{\"alg\":\"ES384\",\"x5u\":\"{b64Spkiy}\"}}";
            string headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));

            var payloadJson = $"{{\"salt\":\"{Convert.ToBase64String(secret)}\"}}";
            string payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            string message = $"{headerBase64}.{payloadBase64}";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] derSignature = ecdsa.SignData(messageBytes, HashAlgorithmName.SHA384);
            string signatureBase64 = Base64UrlEncode(derSignature);
            return $"{message}.{signatureBase64}";
        }


        public static byte[] DecodeBase64Url(string base64Url)
        {
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        public static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
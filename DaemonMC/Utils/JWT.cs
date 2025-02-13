using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace DaemonMC.Utils
{
    public class JWTObject
    {
        public List<string> Chain { get; set; }
    }

    public class JWT
    {
        public const string RootKey = "MHYwEAYHKoZIzj0CAQYFK4EEACIDYgAECRXueJeTDqNRRgJi/vlRufByu/2G0i2Ebt6YMar5QX/R0DIIyrJMcUpruK4QveTfJSTp3Shlq4Gk34cD/4GUWwkv0DVuzeuB+tXija7HBxii03NHDbPAD0AKnLr2wdAp";

        public static void processJWTchain(string jsonString, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            JWTObject decodedObject = JsonConvert.DeserializeObject<JWTObject>(jsonString);
            var handler = new JwtSecurityTokenHandler();

            if (decodedObject == null)
            {
                return;
            }

            foreach (var jwtToken in decodedObject.Chain)
            {
                var jsonToken = handler.ReadToken(jwtToken) as JwtSecurityToken;
                var x5u = jsonToken.Header["x5u"].ToString();

                if (jsonToken != null)
                {
                    if (x5u == RootKey)
                    {
                        Log.debug("Mojang RootKey: OK");
                    }

                    var extraDataClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "extraData");
                    if (extraDataClaim != null)
                    {
                        ExtraData extraData = JsonConvert.DeserializeObject<ExtraData>(extraDataClaim.Value);
                        player.username = extraData.displayName;
                        player.identity = extraData.identity;
                        player.XUID = extraData.XUID;
                    }
                    else
                    {
                        //Disconnect if online mode false
                    }
                }
                else
                {
                    Log.error("Failed to decode JWT.");
                }
            }
        }

        public static void processJWTtoken(string rawToken, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            int index = rawToken.IndexOf("ey");
            string[] tokenParts = rawToken.Substring(index).Split('.');

            string headerJson = DecodeBase64Url(tokenParts[0]);
            string payloadJson = DecodeBase64Url(tokenParts[1]);

            JObject header = JObject.Parse(headerJson);
            JwtPayload payload = JsonConvert.DeserializeObject<JwtPayload>(payloadJson);

            string publicKey = header["x5u"].ToString();

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
                    message = $"Skin decoding failed"
                };
                packet.Encode(encoder);
                Log.error($"Skin decoding failed: {ex.Message}");
            }

            Log.debug($"Public Key (x5u): {publicKey}");
            Log.info($"{player.username} with client version {payload.GameVersion} doing login...");
        }

        private static string DecodeBase64Url(string base64Url)
        {
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            byte[] data = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(data);
        }
    }
}
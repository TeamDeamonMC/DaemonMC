using DaemonMC.Network.Bedrock;
using DaemonMC.Utils;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DaemonMC.Network.Handler
{
    public class LoginHandler
    {
        public static void execute(Login packet, IPEndPoint clientEp)
        {
            byte[] jwtBuffer = Encoding.UTF8.GetBytes(packet.Request);

            string pattern = @"{""chain"":\[.*?\]}";
            var match = Regex.Match(packet.Request, pattern);

            var filteredJWT = match.Value;
            int tokenStartIndex = Encoding.UTF8.GetBytes(filteredJWT).Length + 8;
            var Token = Encoding.UTF8.GetString(jwtBuffer, tokenStartIndex, jwtBuffer.Length - tokenStartIndex);

            JWT.processJWTchain(filteredJWT, clientEp);
            JWT.processJWTtoken(Token, clientEp);

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new PlayStatus
            {
                Status = 0,
            };
            pk.EncodePacket(encoder);
        }
    }
}

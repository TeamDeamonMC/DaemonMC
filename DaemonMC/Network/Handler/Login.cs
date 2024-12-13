using DaemonMC.Network.Bedrock;
using DaemonMC.Utils;
using System.Text;
using System.Text.RegularExpressions;

namespace DaemonMC.Network.Handler
{
    public class Login
    {
        public static void execute(LoginPacket packet)
        {
            byte[] jwtBuffer = Encoding.UTF8.GetBytes(packet.request);

            string pattern = @"{""chain"":\[.*?\]}";
            var match = Regex.Match(packet.request, pattern);

            var filteredJWT = match.Value;
            int tokenStartIndex = Encoding.UTF8.GetBytes(filteredJWT).Length + 8;
            var Token = Encoding.UTF8.GetString(jwtBuffer, tokenStartIndex, jwtBuffer.Length - tokenStartIndex);

            JWT.processJWTchain(filteredJWT);
            JWT.processJWTtoken(Token);

            var pk = new PlayStatusPacket
            {
                status = 0,
            };
            PlayStatus.Encode(pk);
        }
    }
}

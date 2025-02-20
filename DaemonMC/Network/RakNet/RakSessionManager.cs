using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.RakNet
{
    public class RakSessionManager
    {
        public static Dictionary<IPEndPoint, RakSession> sessions = new Dictionary<IPEndPoint, RakSession>();

        public static void createSession(IPEndPoint ip)
        {
            if (sessions.TryGetValue(ip, out var session))
            {
                return;
            }
            sessions.Add(ip, new RakSession());
        }

        public static void addSession(IPEndPoint ip, long guid)
        {
            if (sessions.TryGetValue(ip, out var session))
            {
                sessions[ip].GUID = guid;
            }
            else
            {
                Log.warn($"Session for {ip.Address.ToString()} not found");
            }
        }

        public static RakSession getSession(IPEndPoint ip)
        {
            if (sessions.TryGetValue(ip, out var session))
            {
                return session;
            }
            else
            {
                Log.warn($"Session for {ip.Address.ToString()} not found");
            }
            return null;
        }

        public static bool deleteSession(IPEndPoint ip)
        {
            var player = getSession(ip);
            if (!sessions.Remove(ip))
            {
                Log.warn($"Couldn't delete session for {ip.Address.ToString()}, session doesn't exist.");
                return false;
            }
            else
            {
                Log.debug($"RakNet session closed for {player.username} with IP {ip.Address}");
                Log.info($"{player.username} Requested disconnect and got disconnected successfully.");
                return true;
            }
        }

        public static void Compression(IPEndPoint ip, bool enable)
        {
            if (!sessions.TryGetValue(ip, out var session))
            {
                Log.warn($"Session doesn't exist for {ip.Address.ToString()}.");
                return;
            }
            sessions[ip].initCompression = enable;
        }

        public static void Close()
        {
            foreach (var player in sessions)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player.Key);
                var packet = new Disconnect
                {
                    message = "Server closed"
                };
                packet.EncodePacket(encoder);
            }
            sessions.Clear();
        }
    }
}

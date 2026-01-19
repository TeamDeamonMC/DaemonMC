using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.RakNet
{
    public class RakSessionManager
    {
        public static Dictionary<IPEndPoint, RakSession> sessions = new Dictionary<IPEndPoint, RakSession>();
        public static Dictionary<IPEndPoint, DateTime> blackList = new Dictionary<IPEndPoint, DateTime>();
        private static TimeSpan _timeoutThreshold = TimeSpan.FromSeconds(10);
        private static TimeSpan _blacklistThreshold = TimeSpan.FromSeconds(30);
        private static Timer _timeoutTimer;
        private static Timer _blacklistTimer;

        public static void Init()
        {
            _timeoutTimer = new Timer(CheckTimeouts, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            _blacklistTimer = new Timer(CheckBlacklist, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        public static void createSession(IPEndPoint ip)
        {
            sessions.TryAdd(ip, new RakSession());
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

        public static bool deleteSession(IPEndPoint ip, string reason = "Requested disconnect")
        {
            var player = getSession(ip);
            if (!sessions.Remove(ip))
            {
                Log.warn($"[RakNet] Couldn't delete session for {ip.Address.ToString()}, session doesn't exist.");
                return false;
            }
            else
            {
                Log.debug($"[RakNet] session closed for {player.username} with IP {ip.Address}", ConsoleColor.DarkYellow);
                Log.info($"{player.username} {reason} and got disconnected successfully.");
                return true;
            }
        }

        public static void Compression(IPEndPoint ip, bool enable)
        {
            if (!sessions.TryGetValue(ip, out var session))
            {
                Log.warn($"[RakNet] Session doesn't exist for {ip.Address.ToString()}.");
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
                    Message = "Server closed"
                };
                packet.EncodePacket(encoder);
            }
            sessions.Clear();
        }

        private static void CheckTimeouts(object state)
        {
            var now = DateTime.UtcNow;
            foreach (var session in sessions.ToList())
            {
                if (session.Value.EntityID != 0)
                {
                    session.Value.Nacks = 0;
                    if (now - session.Value.LastPingTime > _timeoutThreshold)
                    {
                        Server.RemovePlayer(getSession(session.Key).EntityID);
                        deleteSession(session.Key, "Timed out");
                    }
                }
            }
        }

        private static void CheckBlacklist(object state)
        {
            var now = DateTime.UtcNow;
            foreach (var ip in blackList)
            {
                if (now - ip.Value > _timeoutThreshold)
                {
                    blackList.Remove(ip.Key);
                    Log.warn($"{ip.Key.Address} IP unblocked");
                }
            }
        }
    }
}

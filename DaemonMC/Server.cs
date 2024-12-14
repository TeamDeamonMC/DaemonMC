using System.Net.Sockets;
using System.Net;
using DaemonMC.Utils.Text;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;

namespace DaemonMC
{
    public class Server
    {
        public static Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();
        public static Socket sock { get; set; } = null!;
        public static int datGrIn = 0;
        public static int datGrOut = 0;

        private static long nextId = 10;
        private static Queue<long> availableIds = new Queue<long>();

        public static long AddPlayer(Player player)
        {
            long id;

            if (availableIds.Count > 0)
            {
                id = availableIds.Dequeue();
            }
            else
            {
                id = nextId++;
            }

            onlinePlayers.Add(id, player);
            Log.debug($"{player.username} has been added to server players with EntityID {id}");
            return id;
        }

        public static bool RemovePlayer(long id)
        {
            if (onlinePlayers.ContainsKey(id))
            {
                var username = GetPlayer(id).username;
                onlinePlayers.Remove(id);
                availableIds.Enqueue(id);
                Log.debug($"Player {username} with EntityID {id} has been removed from the server.");
                return true;
            }
            Log.error($"No player found with EntityID {id}");
            return false;
        }

        public static Player GetPlayer(long id)
        {
            if (onlinePlayers.ContainsKey(id))
            {
                onlinePlayers.TryGetValue(id, out Player player);
                return player;
            }
            Log.error($"No player found with EntityID {id}");
            return null;
        }

        public static void ServerF()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 19132);
            sock.Bind(iep);
            if (Log.debugMode) { Log.warn("Decreased performance expected due to enabled debug mode (DaemonMC.yaml: debug)"); }
            Log.info("Server listening on port 19132");

            Thread titleMonitor = new Thread(titleUpdate);
            titleMonitor.Start();

            while (true)
            {
                EndPoint ep = iep;
                byte[] buffer = new byte[8192];
                int recv = sock.ReceiveFrom(buffer, ref ep);

                if (recv != 0)
                {
                    var client = (IPEndPoint)ep;
                    RakSessionManager.createSession(client);

                    PacketDecoder decoder = PacketDecoderPool.Get(buffer, client);
                    decoder.RakDecoder(decoder, recv);
                }
            }
        }

        public static void Send(byte[] trimmedBuffer, IPEndPoint client)
        {
            sock.SendTo(trimmedBuffer, client);
        }

        private static void titleUpdate()
        {
            while (true)
            {
                Console.Title = $"DaeamonMC | Players {onlinePlayers.Count}/{DaemonMC.maxOnline} | DatGr/sec in:{datGrIn} out:{datGrOut} | Pool cache(in-use) in:{PacketDecoderPool.cached}({PacketDecoderPool.inUse}) out:{PacketEncoderPool.cached}({PacketEncoderPool.inUse})";
                datGrIn = 0;
                datGrOut = 0;
                Thread.Sleep(1000);
            }
        }
    }
}

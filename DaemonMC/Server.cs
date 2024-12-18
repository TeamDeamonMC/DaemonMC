using System.Net.Sockets;
using System.Net;
using DaemonMC.Utils.Text;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;

namespace DaemonMC
{
    public class Server
    {
        public static Socket sock { get; set; } = null!;
        public static Level.Level level { get; set; } = null!;
        public static long nextId = 10;
        public static Queue<long> availableIds = new Queue<long>();
        public static int datGrIn = 0;
        public static int datGrOut = 0;

        public static void ServerF()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 19132);
            sock.Bind(iep);
            if (Log.debugMode) { Log.warn("Decreased performance expected due to enabled debug mode (DaemonMC.yaml: debug)"); }

            level = new Level.Level("level");

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
                Console.Title = $"DaeamonMC | Players {Server.level.onlinePlayers.Count}/{DaemonMC.maxOnline} | DatGr/sec in:{datGrIn} out:{datGrOut} | Pool cache(in-use) in:{PacketDecoderPool.cached}({PacketDecoderPool.inUse}) out:{PacketEncoderPool.cached}({PacketEncoderPool.inUse})";
                datGrIn = 0;
                datGrOut = 0;
                Thread.Sleep(1000);
            }
        }
    }
}

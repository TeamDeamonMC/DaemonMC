using System.Net.Sockets;
using System.Net;
using DaemonMC.Utils.Text;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;
using DaemonMC.Level;
using DaemonMC.Plugin.Plugin;
using DaemonMC.Blocks;

namespace DaemonMC
{
    public class Server
    {
        public static Socket sock { get; set; } = null!;
        public static Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();
        public static long nextId = 10;
        public static Queue<long> availableIds = new Queue<long>();
        public static int datGrIn = 0;
        public static int datGrOut = 0;
        public static int nack = 0;
        public static int rsent = 0;
        public static bool crash = false;
        public static List<World> levels = new List<World>();
        public static List<ResourcePack> packs = new List<ResourcePack>();

        public static void ServerF()
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 19132);
            sock.Bind(iep);
            if (Log.debugMode) { Log.warn("Decreased performance expected due to enabled debug mode (DaemonMC.yaml: debug)"); }

            Palette.buildPalette();

            WorldManager.LoadWorlds("Worlds");

            ResourcePackManager.LoadPacks("Resource Packs");

            CommandManager.RegisterBuiltinCommands();

            PluginManager.LoadPlugins("Plugins");

            Log.info("Server listening on port 19132");
            Log.line();
            Log.info("Type /help to see available commands");

            Thread titleMonitor = new Thread(titleUpdate);
            titleMonitor.Start();

            while (!crash)
            {
                EndPoint ep = iep;
                try
                {
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
                catch (SocketException ex)
                {
                    Log.error($"SocketException: {ex.Message}");
                }
            }

            RakSessionManager.Close();
            Log.error("Server closed because of fatal error");
            ServerClose();
        }

        public static void ServerClose()
        {
            Log.line();
            Log.info("Shutting down...");
            PluginManager.UnloadPlugins();
            foreach (var level in levels)
            {
                level.unload();
            }
            Thread.Sleep(2000);
        }

        public static long AddPlayer(Player player, IPEndPoint ep)
        {
            long id;

            if (Server.availableIds.Count > 0)
            {
                id = availableIds.Dequeue();
            }
            else
            {
                id = nextId++;
            }

            player.ep = ep;
            onlinePlayers.Add(id, player);
            Log.debug($"{player.Username} has been added to server players with EntityID {id}");
            return id;
        }

        public static bool RemovePlayer(long id)
        {
            if (onlinePlayers.ContainsKey(id))
            {
                var player = GetPlayer(id);
                if (player.currentWorld != null)
                {
                    player.currentWorld.removePlayer(player);
                }

                onlinePlayers.Remove(id);
                availableIds.Enqueue(id);
                Log.debug($"Bedrock session closed for {player.Username} with EntityID {id}");
                return true;
            }
            Log.error($"No player found with EntityID {id}");
            return false;
        }

        public static World GetWorld(string WorldName)
        {
            World world = levels.FirstOrDefault(w => w.levelName == WorldName);
            if (world != null)
            {
                return world;
            }
            else
            {
                Log.error($"World with name {WorldName} not found");
                return null;
            }
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

        public static Player GetPlayer(IPEndPoint ep)
        {
            return GetPlayer(RakSessionManager.getSession(ep).EntityID);
        }

        public static Player[] GetOnlinePlayers()
        {
            return onlinePlayers.Values.ToArray();
        }

        private static void titleUpdate()
        {
            while (true)
            {
                Console.Title = $"DaeamonMC | Players {onlinePlayers.Count}/{DaemonMC.maxOnline} | DatGr/sec in:{datGrIn} out:{datGrOut} resend:{rsent} | NACK/sec in:{nack} | Pool cache(in-use) in:{PacketDecoderPool.cached}({PacketDecoderPool.inUse}) out:{PacketEncoderPool.cached}({PacketEncoderPool.inUse})";
                datGrIn = 0;
                datGrOut = 0;
                nack = 0;
                rsent = 0;
                Thread.Sleep(1000);
            }
        }
    }

    public class IPAddressInfo
    {
        public byte[] IPAddress { get; set; }
        public ushort Port { get; set; }
    }

}

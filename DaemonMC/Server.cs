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
        public static Socket Sock { get; set; } = null!;
        public static Dictionary<long, Player> OnlinePlayers = new Dictionary<long, Player>();
        public static long NextId = 10;
        public static Queue<long> AvailableIds = new Queue<long>();
        public static int DatGrIn = 0;
        public static int DatGrOut = 0;
        public static int Nack = 0;
        public static int Rsent = 0;
        public static bool Crash = false;
        public static List<World> Levels = new List<World>();
        public static List<ResourcePack> Packs = new List<ResourcePack>();

        public static void ServerF()
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 19132);
            Sock.Bind(iep);
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

            while (!Crash)
            {
                EndPoint ep = iep;
                try
                {
                    byte[] buffer = new byte[8192];
                    int recv = Sock.ReceiveFrom(buffer, ref ep);

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
            foreach (var level in Levels)
            {
                level.Unload();
            }
            Thread.Sleep(2000);
        }

        public static long AddPlayer(Player player, IPEndPoint ep)
        {
            long id;

            if (Server.AvailableIds.Count > 0)
            {
                id = AvailableIds.Dequeue();
            }
            else
            {
                id = NextId++;
            }

            player.ep = ep;
            OnlinePlayers.Add(id, player);
            Log.debug($"{player.Username} has been added to server players with EntityID {id}");
            return id;
        }

        public static bool RemovePlayer(long id)
        {
            if (OnlinePlayers.ContainsKey(id))
            {
                var player = GetPlayer(id);
                if (player.CurrentWorld != null)
                {
                    player.CurrentWorld.RemovePlayer(player);
                }

                OnlinePlayers.Remove(id);
                AvailableIds.Enqueue(id);
                Log.debug($"Bedrock session closed for {player.Username} with EntityID {id}");
                return true;
            }
            Log.error($"No player found with EntityID {id}");
            return false;
        }

        public static World GetWorld(string WorldName)
        {
            World world = Levels.FirstOrDefault(w => w.LevelName == WorldName);
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
            if (OnlinePlayers.ContainsKey(id))
            {
                OnlinePlayers.TryGetValue(id, out Player player);
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
            return OnlinePlayers.Values.ToArray();
        }

        private static void titleUpdate()
        {
            while (true)
            {
                Console.Title = $"DaeamonMC | Players {OnlinePlayers.Count}/{DaemonMC.MaxOnline} | DatGr/sec in:{DatGrIn} out:{DatGrOut} resend:{Rsent} | NACK/sec in:{Nack} | Pool cache(in-use) in:{PacketDecoderPool.Cached}({PacketDecoderPool.InUse}) out:{PacketEncoderPool.cached}({PacketEncoderPool.inUse})";
                DatGrIn = 0;
                DatGrOut = 0;
                Nack = 0;
                Rsent = 0;
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

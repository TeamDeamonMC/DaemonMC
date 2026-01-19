using System.Net;
using System.Net.Sockets;
using DaemonMC.Blocks;
using DaemonMC.Items;
using DaemonMC.Level;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;
using DaemonMC.Plugin;
using DaemonMC.Utils.Text;

namespace DaemonMC
{
    public class Server
    {
        public static ushort Port { get; set; } = 19132;
        public static IPEndPoint iep { get; set; }
        public static Socket Sock { get; set; } = null!;
        public static Dictionary<long, Player> OnlinePlayers = new Dictionary<long, Player>();
        public static long NextId = 10;
        public static Queue<long> AvailableIds = new Queue<long>();
        public static int DatGrIn = 0;
        public static int DatGrOut = 0;
        public static int NackIn = 0;
        public static int NackOut = 0;
        public static int AckIn = 0;
        public static int AckOut = 0;
        public static int Rsent = 0;
        public static bool Crash = false;
        public static List<World> Levels = new List<World>();
        public static List<ResourcePack> Packs = new List<ResourcePack>();
        private static byte[] bufferAlloc = new byte[8192];

        public static void ServerF()
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            iep = new IPEndPoint(IPAddress.Any, Port);

            try
            {
                Sock.Bind(iep);
            }
            catch (SocketException e)
            {
                if(e.ErrorCode == 10048)
                {
                    Log.error($"Server can't start. Port {Port} is already in use.");
                    Crash = true;
                }
            }

            if (Log.debugMode) { Log.warn("Decreased performance expected due to enabled debug mode (DaemonMC.yaml: debug)"); }

            BlockPalette.buildPalette();

            ItemPalette.buildPalette();

            WorldManager.LoadWorlds("Worlds");

            ResourcePackManager.LoadPacks("Resource Packs");

            CommandManager.RegisterBuiltinCommands();

            PluginManager.LoadPlugins("Plugins");

            Log.info($"Server listening on port {Port}");
            Log.line();
            Log.info("Type /help to see available commands");

            Thread titleMonitor = new Thread(titleUpdate);
            titleMonitor.Start();

            RakSessionManager.Init();

            while (!Crash)
            {
                EndPoint ep = iep;
                try
                {
                    Span<byte> spanBuffer = bufferAlloc.AsSpan();

                    int recv = Sock.ReceiveFrom(bufferAlloc, ref ep);

                    if (recv != 0)
                    {
                        var receivedData = spanBuffer.Slice(0, recv);

                        var client = (IPEndPoint)ep;

                        if (RakSessionManager.blackList.ContainsKey(client))
                        {
                            Log.warn($"Refused connection from blocked IP {client.Address}");
                            return;
                        }

                        RakSessionManager.createSession(client);

                        PacketDecoder decoder = PacketDecoderPool.Get(receivedData.ToArray(), client);
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
            foreach (Player player in OnlinePlayers.Values)
            {
                player.Kick("Server closed");
            }
            Thread.Sleep(1000);
            Log.line();
            Log.info("Shutting down...");
            PluginManager.UnloadPlugins();
            foreach (var level in Levels)
            {
                level.Unload();
            }
            Thread.Sleep(2000);
            Log.InitMsg(false);
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
                    PluginManager.PlayerLeaved(player);
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

        public static Player GetPlayer(string username)
        {
            var player = OnlinePlayers
                .Values
                .FirstOrDefault(p => p.Username == username);

            if (player != null)
            {
                return player;
            }

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
                Console.Title = $"DaeamonMC | Players: {OnlinePlayers.Count}/{DaemonMC.MaxOnline} | DatGr/sec in:{DatGrIn} out:{DatGrOut} resend:{Rsent} | NACK/sec in:{NackIn} out:{NackOut} | ACK/sec in:{AckIn} out:{AckOut}";
                DatGrIn = 0;
                DatGrOut = 0;
                AckIn = 0;
                AckOut = 0;
                NackIn = 0;
                NackOut = 0;
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

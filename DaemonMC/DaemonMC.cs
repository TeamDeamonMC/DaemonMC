using DaemonMC.Utils.Text;
using DaemonMC.Network;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils.Game;
using System.Numerics;
using System.Reflection;
namespace DaemonMC
{
    public static class DaemonMC
    {
        public static string Servername = "DaemonMC";
        public static string Worldname = "Nice new server";
        public static string MaxOnline = "10";
        public static string DefaultWorld = "My World";
        public static int DrawDistance = 10;
        internal static string Version = "unknown";
        internal static string GitHash = "unknown";
        public static void Main()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            var versionInfo = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            Version = versionInfo == null ? "unknown" : versionInfo.Split('+')[0];
            GitHash = versionInfo == null ? "unknown" : versionInfo.Split('+')[1];

            Console.WriteLine(" _____                                ______   ______ ");
            Console.WriteLine("(____ \\                              |  ___ \\ / _____)");
            Console.WriteLine(" _   \\ \\ ____  ____ ____   ___  ____ | | _ | | /      ");
            Console.WriteLine("| |   | / _  |/ _  )    \\ / _ \\|  _ \\| || || | |      ");
            Console.WriteLine("| |__/ ( ( | ( (/ /| | | | |_| | | | | || || | \\_____ ");
            Console.WriteLine($"|_____/ \\_||_|\\____)_|_|_|\\___/|_| |_|_||_||_|\\______) {Version}");
            Console.WriteLine("");
            Log.info($"Setting up server for {MaxOnline} players with Minecraft {Info.Version}");

            Config.Set();

            Thread serverThread = new Thread(new ThreadStart(Server.ServerF));
            serverThread.Start();

            Command();
        }

        static void OnExit(object? sender, EventArgs e)
        {
            Server.ServerClose();
        }

        static void Command()
        {
            string cmd = Console.ReadLine();
            switch (cmd)
            {
                case "/help":
                    Log.line();
                    Log.info("/shutdown - Close server");
                    Log.info("/dev - Debugging mode");
                    Log.info("/list - Player list");
                    Log.info("/liste - Entity list");
                    Log.line();
                    break;
                case "/list":
                    Playerlist();
                    break;
                case "/liste":
                    Entitylist();
                    break;
                case "/shutdown":
                    Server.ServerClose();
                    break;
                case "/dev":
                    Log.line();
                    Log.warn("================== DaemonMC Debugging mode ==================");
                    Log.line();
                    Log.warn("Warning: These commands are only for testing and shouldn't be used for production servers");
                    Log.info("Available commands:");
                    Log.line();
                    Log.info("/exit - Leave debugging mode");
                    Log.info("/actorflags <EntityID (long)> <ActorFlags (int)> - Send specific actorflags value to entity or player");
                    Log.info("/levelevent <Position (X Y Z)> <LevelEvents (int)> - Send specific level event to players");
                    Log.info("/list - Spawned entities list");
                    Log.info("/pklog <enable (bool)> - Enable packet logger");
                    Command2();
                    break;
                default:
                    Log.error("Unknown command. Type /help to see available commands");
                    Log.line();
                    break;
            }
            Command();
        }

        static void Playerlist()
        {
            string table = $"Currently {Server.OnlinePlayers.Count} players on the server\n\n";

            table += string.Format("{0,-15} {1,-15} {2,-10}\n", "Player", "EntityID", "World");
            table += new string('-', 45) + "\n";

            foreach (var player in Server.OnlinePlayers.Values)
            {
                table += string.Format("{0,-15} {1,-15} {2,-10}\n", player.Username, player.EntityID, player.CurrentWorld.LevelName);
            }

            Log.info(table);
        }

        static void Entitylist()
        {
            string table = "\n\n";

            table += string.Format("{0,-35} {1,-15} {2,-10}\n", "Entity", "EntityID", "World");
            table += new string('-', 65) + "\n";

            foreach (var world in Server.Levels)
            {
                foreach (var entity in world.Entities.Values)
                {
                    table += string.Format("{0,-35} {1,-15} {2,-10}\n", entity.ActorType, entity.EntityId, world.LevelName);
                }
                foreach (var entity in world.OnlinePlayers.Values)
                {
                    table += string.Format("{0,-35} {1,-15} {2,-10}\n", $"minecraft:player ({entity.Username})", entity.EntityID, world.LevelName);
                }
            }

            Log.info(table);
        }

        static void Command2()
        {
            string cmd = Console.ReadLine();
            string[] parts = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string command = cmd == "" ? "" : parts[0].ToLower();
            switch (command)
            {
                case "/exit":
                    Log.warn("Leaving Debugging mode");
                    Command();
                    break;
                case "/list":
                    Entitylist();
                    break;
                case "/pklog":
                    if (parts.Length == 2 && bool.TryParse(parts[1], out bool boolValue))
                    {
                        Log.pkLog = boolValue;
                    }
                    else
                    {
                        Log.error("Invalid usage. /pklog <Enable (bool)>");
                    }
                    break;
                case "/actorflags":
                    if (parts.Length == 3 && int.TryParse(parts[1], out int entityID) && int.TryParse(parts[2], out int actorDataId2))
                    {
                        SendMetadata2(actorDataId2, entityID);
                    }
                    else
                    {
                        Log.error("Invalid usage. /actorflags <EntityID (long)> <ActorFlags (int)>");
                    }
                    break;
                case "/levelevent":
                    if (parts.Length == 5 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y) && int.TryParse(parts[3], out int z) && int.TryParse(parts[4], out int eventData))
                    {
                        SendLevelEvent(eventData, new Vector3(x, y, z));
                    }
                    else
                    {
                        Log.error("Invalid usage. /levelevent <Position (X Y Z)> <LevelEvents (int)>");
                    }
                    break;
                default:
                    Log.error("Unknown command");
                    Log.line();
                    break;
            }
            Command2();
        }

        public static void SendLevelEvent(int value, Vector3 pos)
        {
            foreach (var dest in Server.OnlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var packet = new LevelEvent
                {
                    EventID = (LevelEvents)value,
                    Position = pos
                };
                packet.EncodePacket(encoder);
            }
            Log.info($"Sent level event {(LevelEvents)value} for all players at position {pos.X}; {pos.Y}; {pos.Z}");
        }

        public static void SendMetadata2(int value, long entityID)
        {
            long dataValue = 0;
            dataValue |= (1L << value);

            foreach (var dest in Server.OnlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var packet = new SetActorData
                {
                    EntityId = entityID,
                    Metadata = new Dictionary<ActorData, Metadata>() { { ActorData.RESERVED_0,  new Metadata(dataValue) } }
                };
                packet.EncodePacket(encoder);
            }
            Log.info($"Sent actorflags {(ActorFlags)value}:true for entity {entityID}");
        }
    }
}

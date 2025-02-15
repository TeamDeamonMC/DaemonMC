using DaemonMC.Utils.Text;
using DaemonMC.Network;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils.Game;
namespace DaemonMC
{
    public static class DaemonMC
    {
        public static string servername = "DaemonMC";
        public static string worldname = "Nice new server";
        public static string maxOnline = "10";
        public static string defaultWorld = "My World";
        public static void Main()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            Console.WriteLine(" _____                                ______   ______ ");
            Console.WriteLine("(____ \\                              |  ___ \\ / _____)");
            Console.WriteLine(" _   \\ \\ ____  ____ ____   ___  ____ | | _ | | /      ");
            Console.WriteLine("| |   | / _  |/ _  )    \\ / _ \\|  _ \\| || || | |      ");
            Console.WriteLine("| |__/ ( ( | ( (/ /| | | | |_| | | | | || || | \\_____ ");
            Console.WriteLine("|_____/ \\_||_|\\____)_|_|_|\\___/|_| |_|_||_||_|\\______)");
            Console.WriteLine("");
            Log.info($"Setting up server for {maxOnline} players with Minecraft {Info.version}");

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
            Log.info("Type /help to see available commands");
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
                    Log.warn("Warning: These commands are only for testing and shouldn't be used for production");
                    Log.info("Available commands:");
                    Log.line();
                    Log.info("/exit - Leave debugging mode");
                    Log.info("/actorflags <EntityID (long)> <ActorFlags (int)> - Send specific actorflags value to entity or player");
                    Log.info("/list - Spawned entities list");
                    Log.info("/pklog <enable (bool)> - Enable packet logger");
                    Command2();
                    break;
                default:
                    Log.error("Unknown command");
                    Log.line();
                    break;
            }
            Command();
        }

        static void Playerlist()
        {
            string table = $"Currently {Server.onlinePlayers.Count} players on the server\n\n";

            table += string.Format("{0,-15} {1,-15} {2,-10}\n", "Player", "EntityID", "World");
            table += new string('-', 45) + "\n";

            foreach (var player in Server.onlinePlayers.Values)
            {
                table += string.Format("{0,-15} {1,-15} {2,-10}\n", player.Username, player.EntityID, player.currentLevel.levelName);
            }

            Log.info(table);
        }

        static void Entitylist()
        {
            string table = "\n\n";

            table += string.Format("{0,-35} {1,-15} {2,-10}\n", "Entity", "EntityID", "World");
            table += new string('-', 65) + "\n";

            foreach (var world in Server.levels)
            {
                foreach (var entity in world.Entities.Values)
                {
                    table += string.Format("{0,-35} {1,-15} {2,-10}\n", entity.ActorType, entity.EntityId, world.levelName);
                }
                foreach (var entity in world.onlinePlayers.Values)
                {
                    table += string.Format("{0,-35} {1,-15} {2,-10}\n", $"minecraft:player ({entity.Username})", entity.EntityID, world.levelName);
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
                default:
                    Log.error("Unknown command");
                    Log.line();
                    break;
            }
            Command2();
        }

        public static void SendMetadata2(int value, long entityID)
        {
            long dataValue = 0;
            dataValue |= (1L << value);

            foreach (var dest in Server.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var packet = new SetActorData
                {
                    EntityId = entityID,
                    Metadata = new Dictionary<ActorData, Metadata>() { { ActorData.RESERVED_0,  new Metadata(dataValue) } }
                };
                packet.Encode(encoder);
            }
            Log.info($"Sent actorflags {(ActorFlags)value}:true for entity {entityID}");
        }
    }
}

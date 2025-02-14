using DaemonMC.Utils.Text;
using DaemonMC.Network;
using DaemonMC.Network.Enumerations;

namespace DaemonMC
{
    public static class DaemonMC
    {
        public static string servername = "DaemonMC";
        public static string worldname = "Nice new server";
        public static string maxOnline = "10";
        public static void Main()
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(OnExit);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            Config.Set();

            Console.WriteLine(" _____                                ______   ______ ");
            Console.WriteLine("(____ \\                              |  ___ \\ / _____)");
            Console.WriteLine(" _   \\ \\ ____  ____ ____   ___  ____ | | _ | | /      ");
            Console.WriteLine("| |   | / _  |/ _  )    \\ / _ \\|  _ \\| || || | |      ");
            Console.WriteLine("| |__/ ( ( | ( (/ /| | | | |_| | | | | || || | \\_____ ");
            Console.WriteLine("|_____/ \\_||_|\\____)_|_|_|\\___/|_| |_|_||_||_|\\______)");
            Console.WriteLine("");
            Log.info($"Setting up server for {maxOnline} players with Minecraft {Info.version}");

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
                    Log.line();
                    break;
                case "/list":
                    Playerlist();
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
                    Log.info("/actorflags <ActorFlags (int)> <enable (bool)> - Modify specific actorflags for players");
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
                case "/pklog":
                    if (parts.Length == 2 && bool.TryParse(parts[1], out bool boolValue))
                    {
                        Log.pkLog = boolValue;
                    }
                    else
                    {
                        Log.error("Invalid usage. /pklog <enable (bool)>");
                    }
                    break;
                case "/actorflags":
                    if (parts.Length == 3 && int.TryParse(parts[1], out int actorDataId) && bool.TryParse(parts[2], out bool boolValue1))
                    {
                        SendMetadata(actorDataId, boolValue1);
                    }
                    else
                    {
                        Log.error("Invalid usage. /actorflags <ActorFlags (int)> <enable (bool)>");
                    }
                    break;
                default:
                    Log.error("Unknown command");
                    Log.line();
                    break;
            }
            Command2();
        }

        public static void SendMetadata(int value, bool enable)
        {
            foreach (var dest in Server.onlinePlayers)
            {
                var player = dest.Value;
                player.setFlag((ActorFlags)value, enable);
                player.SendMetadata();
                Log.info($"Updated actorflags {(ActorFlags)value}:{enable} for {player.Username}");
            }
        }
    }
}

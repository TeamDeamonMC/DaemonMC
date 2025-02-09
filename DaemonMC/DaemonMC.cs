using DaemonMC.Utils.Text;
using DaemonMC.Network;

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

            //Console.WriteLine("Choose DaemonMC mode");
            //Console.WriteLine("1 - Server");
            //Console.WriteLine("2 - Client");
            //string mode = Console.ReadLine();
            var mode = "1";
            if (mode == "1")
            {
                Server.ServerF();
            }else if (mode == "2")
            {
                //Client.ClientF();
            }
            else
            {
                Console.WriteLine("unknown mode");
                Console.WriteLine("");
                Main();
            }
        }

        static void OnExit(object? sender, EventArgs e)
        {
            Server.ServerClose();
        }
    }
}

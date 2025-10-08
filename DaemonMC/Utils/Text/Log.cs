using System.Net;
using DaemonMC.Network;

namespace DaemonMC.Utils.Text
{
    public class Log
    {
        private static string currentLogFile;
        private static StreamWriter writer;

        public static List<Info.Bedrock> ignoredPackets = new List<Info.Bedrock> {
            Info.Bedrock.PlayerAuthInput,
            Info.Bedrock.LevelChunk
        };

        public static List<Info.RakNet> ignoredRakPackets = new List<Info.RakNet> {
            Info.RakNet.ACK
        };

        public static bool debugMode = false;
        public static bool pkLog = false;
        public static bool raLog = false;
        public static void debug(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            if (debugMode)
            {
                Console.ForegroundColor = color;
                ToLog($"[DEBUG] {message}");
                Console.ResetColor();
            }
        }
        public static void packetIn(IPEndPoint clientEp, Info.RakNet id)
        {
            if (ignoredRakPackets.Contains(id) || !raLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            ToLog($"[Server] <-- [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void packetOut(IPEndPoint clientEp, Info.RakNet id)
        {
            if (ignoredRakPackets.Contains(id) || !raLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            ToLog($"[Server] --> [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void packetIn(IPEndPoint clientEp, Info.Bedrock id)
        {
            if (ignoredPackets.Contains(id) || !pkLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            ToLog($"[Server] <-- [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void packetOut(IPEndPoint clientEp, Info.Bedrock id)
        {
            if (ignoredPackets.Contains(id) || !pkLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            ToLog($"[Server] --> [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void debug(int message)
        {
            debug(message.ToString());
        }

        public static void info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ToLog($"[INFO] {message}");
            Console.ResetColor();
        }

        public static void info(int message)
        {
            info(message.ToString());
        }

        public static void warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            ToLog($"[WARN] {message}");
            Console.ResetColor();
        }

        public static void warn(int message)
        {
            warn(message.ToString());
        }

        public static void error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ToLog($"[ERROR] {message}");
            Console.ResetColor();
        }

        public static void error(int message)
        {
            error(message.ToString());
        }

        public static void line()
        {
            Console.WriteLine();
        }

        public static void ToLog(string message, bool print = true)
        {
            UpdateLogFile();
            Console.WriteLine(message);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine($"[{timestamp}] {message}");
        }

        public static void InitMsg(bool startup)
        {
            UpdateLogFile();
            string state = startup ? "started" : "shutdown";
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine($"============= Server {state} at {timestamp} =============");
        }

        private static void UpdateLogFile()
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string logFile = $"{date}.log";

            if (logFile != currentLogFile)
            {
                writer?.Dispose();
                currentLogFile = logFile;
                writer = new StreamWriter(currentLogFile, append: true)
                {
                    AutoFlush = true
                };
            }
        }
    }
}

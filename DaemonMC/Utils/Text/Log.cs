namespace DaemonMC.Utils.Text
{
    public class Log
    {
        public static bool debugMode = false;
        public static void debug(string message)
        {
            if (debugMode)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"[DEBUG] {message}");
                Console.ResetColor();
            }
        }

        public static void debug(int message)
        {
            debug(message.ToString());
        }

        public static void info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] {message}");
            Console.ResetColor();
        }

        public static void info(int message)
        {
            info(message.ToString());
        }

        public static void warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] {message}");
            Console.ResetColor();
        }

        public static void warn(int message)
        {
            warn(message.ToString());
        }

        public static void error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {message}");
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
    }
}

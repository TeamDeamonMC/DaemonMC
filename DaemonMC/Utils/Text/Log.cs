﻿using System.Net;
using DaemonMC.Network;

namespace DaemonMC.Utils.Text
{
    public class Log
    {
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
                Console.WriteLine($"[DEBUG] {message}");
                Console.ResetColor();
            }
        }
        public static void packetIn(IPEndPoint clientEp, Info.RakNet id)
        {
            if (ignoredRakPackets.Contains(id) || !raLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[Server] <-- [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void packetOut(IPEndPoint clientEp, Info.RakNet id)
        {
            if (ignoredRakPackets.Contains(id) || !raLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[Server] --> [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void packetIn(IPEndPoint clientEp, Info.Bedrock id)
        {
            if (ignoredPackets.Contains(id) || !pkLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[Server] <-- [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
        }

        public static void packetOut(IPEndPoint clientEp, Info.Bedrock id)
        {
            if (ignoredPackets.Contains(id) || !pkLog) { return; }
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[Server] --> [{clientEp.Address,-16}:{clientEp.Port}] {id}");
            Console.ResetColor();
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

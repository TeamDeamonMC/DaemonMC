using System.Diagnostics;
using System.Runtime.InteropServices;
using DaemonMC.Plugin;
using DaemonMC.Utils.Game;

namespace DaemonMC.Plugin.Core;

[PluginInfo("ServerInfoCommand", "1.0.0")]
public class ServerInfoCommand : Plugin
{
    private const string MainCommand = "serverinfo";
    private const string AliasCommand = "stats";

    public override void OnLoad()
    {
        RegisterCommand(MainCommand);
        RegisterCommand(AliasCommand);
    }

    public override void OnUnload()
    {
        CommandManager.Unregister(MainCommand);
        CommandManager.Unregister(AliasCommand);
    }

    private static void RegisterCommand(string name)
    {
        CommandManager.Register(
            new Command(name, "Shows server, process, and memory statistics"),
            Execute);
    }

    private static void Execute(CommandAction action)
    {
        var player = action.Player;
        var process = Process.GetCurrentProcess();
        process.Refresh();

        var uptime = DateTime.UtcNow - Server.StartedAtUtc;
        var cpuTime = process.TotalProcessorTime;
        var cpuPercent = uptime.TotalMilliseconds > 0
            ? cpuTime.TotalMilliseconds / (uptime.TotalMilliseconds * Environment.ProcessorCount) * 100.0
            : 0.0;

        var message =
            "§k§r§7§lDaemon§8MC§r§f Server Info\n" +
            $"§fServer: §a{DaemonMC.Servername} §7({DaemonMC.Version}, {DaemonMC.GitHash})\n" +
            $"§fUptime: §a{FormatTimeSpan(uptime)} §7| §fStart: §a{Server.StartedAtUtc:u}\n" +
            $"§fPlayers: §a{Server.OnlinePlayers.Count}§7/§a{DaemonMC.MaxOnline} §fWorlds: §a{Server.Levels.Count} §fPacks: §a{Server.Packs.Count}\n" +
            $"§fCPU: §a{cpuPercent:F1}% average §7| §fCores: §a{Environment.ProcessorCount} §7| §fThreads: §a{process.Threads.Count}\n" +
            $"§fMemory: §a{FormatBytes(process.WorkingSet64)} §7working set, §a{FormatBytes(process.PrivateMemorySize64)} §7private, §a{FormatBytes(GC.GetTotalMemory(false))} §7managed\n" +
            $"§fOS: §a{RuntimeInformation.OSDescription} §7({RuntimeInformation.OSArchitecture}) §fRuntime: §a.NET {Environment.Version}\n" +
            $"§fPort: §a{Server.Port} §7| §fProcess ID: §a{process.Id}";

        player.SendMessage(message);
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        return timeSpan.TotalDays >= 1
            ? $"{(int)timeSpan.TotalDays}d {timeSpan:hh\\:mm\\:ss}"
            : timeSpan.ToString(@"hh\:mm\:ss");
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = { "B", "KiB", "MiB", "GiB", "TiB" };
        double size = bytes;
        int unit = 0;

        while (size >= 1024 && unit < units.Length - 1)
        {
            size /= 1024;
            unit++;
        }

        return $"{size:0.0} {units[unit]}";
    }
}

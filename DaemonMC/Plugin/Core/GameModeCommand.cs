using DaemonMC.Plugin;
using DaemonMC.Utils.Game;

namespace DaemonMC.Plugin.Core;

[PluginInfo("GameModeCommand", "1.0.0")]
public class GameModeCommand : Plugin
{
    private const string MainCommand = "gamemode";
    private const string AliasCommand = "gm";

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
            new Command(name, "Changes your game mode", 2, new List<Parameter> { new StringP("mode") }),
            ExecuteGameMode);
    }

    private static void ExecuteGameMode(CommandAction action)
    {
        var player = action.Player;
        var targetMode = ParseGameMode(action.Data.Length > 0 ? action.Data[0] as string : null);

        if (targetMode == null)
        {
            player.SendMessage("§cUsage: /gamemode <survival|adventure|creative>");
            return;
        }

        player.SetGameMode(targetMode.Value);

        if (player.Abilities.Count == 0)
        {
            player.Abilities.Add(new AbilitiesData(1, 262143, new PermissionSet(), 0.05f, 0.1f, 0.1f));
        }

        player.Abilities[0].AbilityValues.MayFly = targetMode.Value == 2;
        player.SendAbilities();

        player.SendMessage($"§aYour game mode is now {GameModeName(targetMode.Value)}.");
    }

    private static int? ParseGameMode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        switch (value.Trim().ToLowerInvariant())
        {
            case "0":
            case "s":
            case "survival":
                return 0;
            case "1":
            case "a":
            case "adventure":
                return 1;
            case "2":
            case "c":
            case "creative":
                return 2;
            default:
                return null;
        }
    }

    private static string GameModeName(int gameMode)
    {
        return gameMode switch
        {
            0 => "survival",
            1 => "adventure",
            2 => "creative",
            _ => "unknown"
        };
    }
}

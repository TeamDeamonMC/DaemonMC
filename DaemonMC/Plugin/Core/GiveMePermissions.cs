using DaemonMC;
using DaemonMC.Plugin;
using DaemonMC.Utils.Game;

namespace DaemonMC.Plugin.Core;

[PluginInfo("GiveMePermissions","1.0.0")]
public class GiveMePermissions : Plugin
{
    private const string CommandName = "op";

    public override void OnLoad()
    {
        CommandManager.Register(
            new Command(CommandName, "Grants the executing player full permissions"),
            GiveFullPermissions);
    }

    public override void OnUnload()
    {
        CommandManager.Unregister(CommandName);
    }

    private static void GiveFullPermissions(CommandAction action)
    {
        var player = action.Player;

        var permissions = new PermissionSet
        {
            Build = true,
            Mine = true,
            DoorsAndSwitches = true,
            OpenContainers = true,
            AttackPlayers = true,
            AttackMobs = true,
            OperatorCommands = true,
            Teleport = true,
            MayFly = true
        };

        if (player.Abilities.Count == 0)
        {
            player.Abilities.Add(new AbilitiesData(1, 262143, permissions, 0.05f, 0.1f, 0.1f));
        }
        else
        {
            player.Abilities[0].AbilityValues = permissions;
        }

        player.SendAbilities(4, 3);
        player.SendMessage("§aYou now have full permissions.");
    }
}

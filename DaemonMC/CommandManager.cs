using DaemonMC.Network;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC
{
    public class CommandManager
    {
        public static List<Command> AvailableCommands = new List<Command>();

        public static void Register(Command command, Action<Player> commandFunction)
        {
            if (AvailableCommands.FirstOrDefault(p => p.Name == command.Name) != null)
            {
                Log.warn($"Couldn't register {command.Name}. Command already registered.");
                return;
            }
            command.CommandFunction = commandFunction;
            AvailableCommands.Add(command);
        }

        public static void Unregister(string commandName)
        {
            var cmd = AvailableCommands.FirstOrDefault(p => p.Name == commandName);
            if (cmd == null)
            {
                Log.warn($"Couldn't unregister {commandName}. Command not found.");
                return;
            }
            AvailableCommands.Remove(cmd);
        }

        public static void Execute(string commandName, Player player)
        {
            var command = AvailableCommands.FirstOrDefault(c => c.Name == commandName);
            command?.CommandFunction?.Invoke(player);
        }

        public static void RegisterBuiltinCommands()
        {
            Register(new Command("about", "Information about the server"), about);
            Register(new Command("pos", "Your current position"), position);
        }

        public static void about(Player player)
        {
            player.SendMessage($"§k§r§7§lDaemon§8MC§r§k§r {DaemonMC.version} \n§r§fProject URL: §agithub.com/laz1444/DaemonMC \n§r§fGit hash: §a{DaemonMC.gitHash} \n§r§fBuild info: §a.NET{Environment.Version} \n§r§fSupported MCBE versions: §a{string.Join(", ", Info.protocolVersion)}");
        }

        public static void position(Player player)
        {
            player.SendMessage($"§fCoordinates: §a{player.Position}");
        }
    }
}

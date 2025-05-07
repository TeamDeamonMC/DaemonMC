using System.Numerics;
using DaemonMC.Network;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC
{
    public class CommandManager
    {
        public static List<Command> AvailableCommands { get; set; } = new List<Command>();
        public static List<string> EnumValues { get; set; } = new List<string>();
        public static List<CommandEnum> RealEnums { get; set; } = new List<CommandEnum>();

        private static readonly Dictionary<Type, ParameterTypes> typeMap = new()
        {
            { typeof(int), ParameterTypes.Int },
            { typeof(float), ParameterTypes.Float },
            { typeof(string), ParameterTypes.Id },
            { typeof(Player), ParameterTypes.Selection },
            { typeof(Vector3), ParameterTypes.PositionFloat },
            { typeof(EnumP), ParameterTypes.Enum },
        };
        private static readonly Dictionary<Type, string> typeStringMap = new()
        {
            { typeof(int), "int" },
            { typeof(float), "float" },
            { typeof(string), "string" },
            { typeof(Player), "target" },
            { typeof(Vector3), "x y z" },
        };

        public static void Register(Command command, Action<CommandAction> commandFunction)
        {
            var regCommand = AvailableCommands.FirstOrDefault(p => p.Name == command.Name);

            if (regCommand != null)
            {
                bool overloadExists = regCommand.Overloads.Any(existingOverload => existingOverload.Count == command.Parameters.Count && !existingOverload.Where((param, index) => param.Name != command.Parameters[index].Name || param.Type != command.Parameters[index].Type).Any());
                if (overloadExists)
                {
                    Log.warn($"Couldn't register {command.Name}. Command already registered.");
                    return;
                }
                else
                {
                    regCommand.Overloads.Add(command.Parameters);
                    regCommand.CommandFunction.Add(commandFunction);
                }
            }
            else
            {
                command.Overloads.Add(command.Parameters);
                command.CommandFunction.Add(commandFunction);
                AvailableCommands.Add(command);
            }

            foreach (var param in command.Parameters)
            {
                if (param is EnumP enumParam)
                {
                    if (RealEnums.FirstOrDefault(p => p.Name == enumParam.Name) == null)
                    {
                        RealEnums.Add(new CommandEnum(enumParam.Name, enumParam.Values.ToList()));
                        EnumValues.AddRange(enumParam.Values.Except(EnumValues));
                    }
                }
            }
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

        public static int GetSymbol(Type type, int enumIndex = -1)
        {
            if (typeMap.TryGetValue(type, out var paramType))
            {
                if (type == typeof(Enum))
                {
                    return (int)ParameterTypes.Enum | enumIndex;
                }
                return (int)ParameterTypes.Epsilon | (int)paramType;
            }

            Log.error($"Unknown command parameter type: {type}");
            return (int)ParameterTypes.Epsilon | (int)ParameterTypes.RawText;
        }

        public static void Execute(string command, Player player)
        {
            if (string.IsNullOrWhiteSpace(command)) return;

            string[] commandParts = command.Split(' ');
            if (commandParts.Length == 0) return;

            string commandName = commandParts[0];
            string[] argParts = commandParts.Skip(1).ToArray();

            var regCommand = AvailableCommands.FirstOrDefault(c => c.Name == commandName);
            if (regCommand == null)
            {
                player.SendMessage($"§cUnknown command: {commandName}");
                return;
            }

            for (int i = 0; i < regCommand.Overloads.Count; i++)
            {
                var overload = regCommand.Overloads[i];
                List<object> parsedArgs = new List<object>();
                int argIndex = 0;
                bool success = true;

                foreach (var param in overload)
                {
                    if (argIndex >= argParts.Length)
                    {
                        success = false;
                        break;
                    }

                    Type expectedType = param.Type;

                    if (expectedType == typeof(Vector3) && Parser.Vector3(argParts, argIndex, out Vector3 vec))
                    {
                        parsedArgs.Add(vec);
                        argIndex += 3;
                    }
                    else if (expectedType == typeof(int) && int.TryParse(argParts[argIndex], out int intVal))
                    {
                        parsedArgs.Add(intVal);
                        argIndex++;
                    }
                    else if (expectedType == typeof(float) && float.TryParse(argParts[argIndex], out float floatVal))
                    {
                        parsedArgs.Add(floatVal);
                        argIndex++;
                    }
                    else if (expectedType == typeof(string) || expectedType == typeof(Player))
                    {
                        parsedArgs.Add(argParts[argIndex]);
                        argIndex++;
                    }
                    else if (expectedType == typeof(EnumP) && param.Values.Contains(argParts[argIndex], StringComparer.OrdinalIgnoreCase))
                    {
                        parsedArgs.Add(argParts[argIndex]);
                        argIndex++;
                    }
                    else
                    {
                        success = false;
                        break;
                    }
                }

                if (success && argIndex == argParts.Length)
                {
                    regCommand.CommandFunction[i]?.Invoke(new CommandAction(player, parsedArgs.ToArray()));
                    return;
                }
            }

            Log.debug($"Failed command request: {command}");
            player.SendMessage("§cIncorrect usage. Available parameters:");
            foreach (var overload in regCommand.Overloads)
            {
                var parameters = string.Join(" ", overload.Select(p => $"<{p.Name}: {(p.Type == typeof(EnumP) ? string.Join(" | ", p.Values) : typeStringMap[p.Type])}>"));
                player.SendMessage($"§f/{regCommand.Name} §a{parameters}");
            }
        }

        public static void RegisterBuiltinCommands()
        {
            Register(new Command("about", "Information about the server"), about);
            Register(new Command("pos", "Your current position"), position);
        }

        public static void about(CommandAction action)
        {
            action.Player.SendMessage($"§k§r§7§lDaemon§8MC§r§k§r {DaemonMC.Version} \n§r§fProject URL: §agithub.com/laz1444/DaemonMC \n§r§fGit hash: §a{DaemonMC.GitHash} \n§r§fEnvironment: §a.NET{Environment.Version}, {Environment.OSVersion} \n§r§fSupported MCBE versions: §a{string.Join(", ", Info.ProtocolVersion)}");
        }

        public static void position(CommandAction action)
        {
            action.Player.SendMessage($"§fCoordinates: §a{action.Player.Position}");
        }
    }

    public class CommandAction
    {
        public Player Player { get; set; }
        public object[] Data { get; set; }

        public CommandAction(Player player, object[] data)
        {
            Player = player;
            Data = data;
        }
    }

    public class CommandEnum
    {
        public string Name { get; set; } = "";
        public List<string> Values { get; set; } = new List<string>();

        public CommandEnum(string name, List<string> values)
        {
            Name = name;
            Values = values;
        }
    }
}

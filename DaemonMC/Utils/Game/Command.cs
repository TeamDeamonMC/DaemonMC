namespace DaemonMC.Utils.Game
{
    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Flags { get; set; }
        public byte Permission { get; set; }
        public int AliasEnum { get; set; }
        public List<short> ChainedSubcommandIndex = new List<short>() { };
        public List<List<Parameter>> Overloads { get; set; } = new List<List<Parameter>>();
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();

        public List<Action<CommandAction>> CommandFunction { get; set; } = new List<Action<CommandAction>>();

        public Command(string name, string description, byte permission = 0, List<Parameter> overloads = null)
        {
            Name = name;
            Description = description;
            Permission = permission;
            Parameters = overloads ?? new List<Parameter>();
        }
    }

    public class Parameter
    {
        public string Name { get; set; } = "";
        public Type Type { get; set; }

        public Parameter(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}

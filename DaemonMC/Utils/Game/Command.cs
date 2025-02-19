namespace DaemonMC.Utils.Game
{
    public class Command
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Flags { get; set; }
        public byte Permission { get; set; }
        public int AliasEnum { get; set; }
        public List<short> ChainedSubcommandIndex = new List<short>();

        public Action<Player> CommandFunction { get; set; }

        public Command(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}

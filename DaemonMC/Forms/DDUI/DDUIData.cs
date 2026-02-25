namespace DaemonMC.Forms.DDUI
{
    public class DDUIData
    {
        public string Name { get; set; } = "";
        public string Property { get; set; } = "";
        public string Path { get; set; } = "";
        public object Value;

        public DDUIData(string name, string property, string path, object value)
        {
            Name = name;
            Property = property;
            Path = path;
            Value = value;
        }
    }
}

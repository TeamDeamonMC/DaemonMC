namespace DaemonMC.Forms.Components
{
    public class Dropdown : Component
    {
        public List<string> Options { get; set; } = new List<string>();
        public int Default { get; set; } = 0;

        public Dropdown(string text, List<string> options, string defaultOption = "")
        {
            var optionIndex = Options.IndexOf(defaultOption);

            Type = "dropdown";
            Text = text;
            Options = options;
            Default = optionIndex == -1 ? 0 : optionIndex;
        }
    }
}

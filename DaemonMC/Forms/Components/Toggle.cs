namespace DaemonMC.Forms.Components
{
    public class Toggle : Component
    {
        public bool Default { get; set; } = false;
        public Toggle(string text, bool defaultValue = false)
        {
            Type = "toggle";
            Text = text;
            Default = defaultValue;
        }
    }
}

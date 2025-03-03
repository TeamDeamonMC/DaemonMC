namespace DaemonMC.Forms.Components
{
    public class Input : Component
    {
        public string Placeholder { get; set; } = "";
        public string Default { get; set; } = "";

        public Input(string text, string placeHolder = "", string defaultValue = "")
        {
            Type = "input";
            Text = text;
            Placeholder = placeHolder;
            Default = defaultValue;
        }
    }
}

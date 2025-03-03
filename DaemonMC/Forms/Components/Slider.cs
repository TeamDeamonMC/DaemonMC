namespace DaemonMC.Forms.Components
{
    public class Slider : Component
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 0;
        public int Step { get; set; } = 0;

        public Slider(string text, int min, int max, int step = 1)
        {
            Type = "slider";
            Text = text;
            Min = min;
            Max = max;
            Step = step;
        }
    }
}

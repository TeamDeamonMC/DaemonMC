namespace DaemonMC.Forms.Components
{
    public class StepSlider : Component
    {
        public List<string> Steps { get; set; } = new List<string>();
        public int Default { get; set; } = 0;

        public StepSlider(string text, List<string> steps, string defaultStep = "")
        {
            var optionIndex = Steps.IndexOf(defaultStep);

            Type = "step_slider";
            Text = text;
            Steps = steps;
            Default = optionIndex == -1 ? 0 : optionIndex;
        }
    }
}

using DaemonMC.Forms.Components;

namespace DaemonMC.Forms
{
    public class CustomForm : Form
    {
        public List<Component> Content { get; set; } = new List<Component>();

        public CustomForm()
        {
            Id = new Random().Next();
            Type = "custom_form";
        }
    }
}
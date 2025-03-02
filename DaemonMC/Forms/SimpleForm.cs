namespace DaemonMC.Forms
{
    public class SimpleForm : Form
    {
        public string Content { get; set; } = "";
        public List<Button> Buttons { get; set; } = new List<Button>();

        public SimpleForm()
        {
            Id = new Random().Next();
            Type = "form";
        }
    }
}
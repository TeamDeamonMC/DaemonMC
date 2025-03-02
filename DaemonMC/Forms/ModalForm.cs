namespace DaemonMC.Forms
{
    public class ModalForm : Form
    {
        public string Content { get; set; } = "";
        public string Button1 { get; set; } = "";
        public string Button2 { get; set; } = "";

        public ModalForm()
        {
            Id = new Random().Next();
            Type = "modal";
        }
    }
}
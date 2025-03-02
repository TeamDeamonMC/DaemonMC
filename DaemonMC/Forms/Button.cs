namespace DaemonMC.Forms
{
    public class Button
    {
        public string Text { get; set; }
        public Image Image { get; set; }

        public Button(string text)
        {
            Text = text;
        }
        public Button(string text, string imagePath)
        {
            Text = text;
            Image = new Image(imagePath.Contains("http") ? "url" : "path", imagePath);
        }
    }

    public class Image
    {
        public string Type { get; set; }
        public string Data { get; set; }

        public Image(string type, string path)
        {
            Type = type;
            Data = path;
        }
    }
}

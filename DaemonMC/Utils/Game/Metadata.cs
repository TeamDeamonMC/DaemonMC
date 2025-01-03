namespace DaemonMC.Utils.Game
{
    public class Metadata
    {
        public object Value { get; set; } = new object();

        public Metadata(object value)
        {
            Value = value;
        }
    }
}

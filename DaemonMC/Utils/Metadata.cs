using DaemonMC.Network.Enumerations;

namespace DaemonMC.Utils
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

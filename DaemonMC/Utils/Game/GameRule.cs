namespace DaemonMC.Utils.Game
{
    public class GameRule
    {
        public object Value { get; set; } = new object();

        public GameRule(object value)
        {
            Value = value;
        }
    }
}

namespace DaemonMC.Utils.Game
{
    public class SynchedProperties
    {
        public Dictionary<int, int> intEntries = new Dictionary<int, int>();
        public Dictionary<int, float> floatEntries = new Dictionary<int, float>();

        public SynchedProperties() { }

        public SynchedProperties(Dictionary<int, int> IntEntries, Dictionary<int, float> FloatEntries)
        {
            intEntries = IntEntries;
            floatEntries = FloatEntries;
        }

        public void UpdateEntry(int Index, int Data)
        {
            intEntries[Index] = Data;
        }

        public void UpdateEntry(int Index, float Data)
        {
            floatEntries[Index] = Data;
        }
    }
}

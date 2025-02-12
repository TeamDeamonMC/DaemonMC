using DaemonMC.Utils;

namespace DaemonMC.Network.RakNet
{
    public class RakSession
    {
        public long GUID { get; set; }
        public string XUID { get; set; } = "";
        public bool initCompression { get; set; }
        public string username { get; set; } = "";
        public string identity { get; set; }
        public long EntityID { get; set; }
        public uint sequenceNumber { get; set; } = 0;
        public int protocolVersion { get; set; } = 0;
        public Skin skin { get; set; } = new Skin();

        public RakSession(bool compression = false)
        {
            initCompression = compression;
        }
    }
}

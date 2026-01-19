using DaemonMC.Utils;

namespace DaemonMC.Network.RakNet
{
    public class RakSession
    {
        public object Sync { get; } = new object();
        public DateTime LastPingTime { get; set; } = DateTime.UtcNow;
        public int MTU { get; set; } = 1500;
        public Encryptor encryptor { get; set; }
        public long GUID { get; set; }
        public string XUID { get; set; } = "";
        public bool initCompression { get; set; }
        public string username { get; set; } = "";
        public string identity { get; set; }
        public string identityPublicKey { get; set; }
        public long EntityID { get; set; }
        public uint sequenceNumber { get; set; } = 0;
        public ushort compId { get; set; } = 0;
        public uint orderIndex { get; set; } = 0;
        public Dictionary<uint, (byte[], bool)> sentPackets { get; set; } = new Dictionary<uint, (byte[], bool)>();
        public int Nacks { get; set; } = 0;
        public int protocolVersion { get; set; } = 0;
        public Skin skin { get; set; } = new Skin();

        public bool isClient = false;
        public Client client { get; set; }

        public RakSession(bool compression = false)
        {
            initCompression = compression;
        }
    }
}

using DaemonMC.Forms.DDUI;

namespace DaemonMC.Network.Bedrock
{
    public class ClientboundDataStore : Packet
    {
        public override int Id => (int) Info.Bedrock.ClientboundDataStore;

        public List<DDUIData> DataStore { get; set; } = new List<DDUIData>();
        public DataStoreType DataType { get; set; } = DataStoreType.Update;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteDDUIData(DataStore, DataType);
        }
    }
}

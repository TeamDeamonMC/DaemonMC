namespace DaemonMC.Network.Bedrock
{
    public class RequestNetworkSettings : Packet
    {
        public override int Id => (int) Info.Bedrock.RequestNetworkSettings;

        public int ProtocolVersion { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            ProtocolVersion = decoder.ReadIntBE();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

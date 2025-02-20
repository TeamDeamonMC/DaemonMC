namespace DaemonMC.Network.Bedrock
{
    public class RequestNetworkSettings : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.RequestNetworkSettings;

        public int protocolVersion = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            protocolVersion = decoder.ReadIntBE();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

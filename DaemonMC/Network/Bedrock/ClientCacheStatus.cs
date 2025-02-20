namespace DaemonMC.Network.Bedrock
{
    public class ClientCacheStatus : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ClientCacheStatus;

        public bool status = false;

        protected override void Decode(PacketDecoder decoder)
        {
            status = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

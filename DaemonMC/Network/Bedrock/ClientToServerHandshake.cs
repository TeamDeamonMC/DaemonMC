namespace DaemonMC.Network.Bedrock
{
    public class ClientToServerHandshake : Packet
    {
        public override int Id => (int) Info.Bedrock.ClientToServerHandshake;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

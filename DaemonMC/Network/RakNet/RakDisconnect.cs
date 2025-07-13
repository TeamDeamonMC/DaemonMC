namespace DaemonMC.Network.RakNet
{
    public class RakDisconnect : Packet
    {
        public override int Id => (int) Info.RakNet.Disconnect;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

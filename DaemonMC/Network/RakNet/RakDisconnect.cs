namespace DaemonMC.Network.RakNet
{
    public class RakDisconnectPacket
    {

    }

    public class RakDisconnect
    {
        public byte id = 21;
        public void Decode(PacketDecoder decoder)
        {
            var packet = new RakDisconnectPacket
            {
            };
            RakPacketProcessor.Disconnect(packet, decoder.endpoint);
        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.handlePacket("raknet");
        }
    }
}

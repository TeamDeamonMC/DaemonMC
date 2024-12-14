namespace DaemonMC.Network.RakNet
{
    public class RakDisconnectPacket
    {

    }

    public class RakDisconnect
    {
        public static byte id = 21;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new RakDisconnectPacket
            {
            };
            RakPacketProcessor.Disconnect(packet, decoder.endpoint);
        }

        public static void Encode(RakDisconnectPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.handlePacket("raknet");
        }
    }
}

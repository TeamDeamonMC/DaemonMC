namespace DaemonMC.Network.RakNet
{
    public class ConnectedPingPacket
    {
        public long Time { get; set; }
    }

    public class ConnectedPing
    {
        public static byte id = 0;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new ConnectedPingPacket
            {
                Time = decoder.ReadLongLE(),
            };

            RakPacketProcessor.ConnectedPing(packet, decoder.clientEp);
        }

        public static void Encode(ConnectedPingPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteLongLE(fields.Time);
            encoder.handlePacket("raknet");
        }
    }
}

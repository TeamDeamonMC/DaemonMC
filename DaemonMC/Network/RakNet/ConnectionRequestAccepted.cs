namespace DaemonMC.Network.RakNet
{
    public class ConnectionRequestAcceptedPacket
    {
        public long Time { get; set; }
    }

    public class ConnectionRequestAccepted
    {
        public static byte id = 16;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(ConnectionRequestAcceptedPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteAddress();
            encoder.WriteShort(0);

            for (int i = 0; i < 20; ++i)
            {
                encoder.WriteAddress();
            }

            encoder.WriteLongLE(fields.Time);
            encoder.WriteLongLE(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            encoder.handlePacket("raknet");
        }
    }
}

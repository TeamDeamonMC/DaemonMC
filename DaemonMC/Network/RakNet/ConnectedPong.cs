namespace DaemonMC.Network.RakNet
{
    public class ConnectedPongPacket
    {
        public long pingTime { get; set; }
        public long pongTime { get; set; }
    }

    public class ConnectedPong
    {
        public static byte id = 3;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(ConnectedPongPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteLongLE(fields.pingTime);
            encoder.WriteLongLE(fields.pongTime);
            encoder.handlePacket("raknet");
        }
    }
}

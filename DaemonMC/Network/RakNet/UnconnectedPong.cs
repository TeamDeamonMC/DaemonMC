namespace DaemonMC.Network.RakNet
{
    public class UnconnectedPongPacket
    {
        public long Time { get; set; }
        public long GUID { get; set; }
        public string Magic { get; set; }
        public string MOTD { get; set; }
    }

    public class UnconnectedPong
    {
        public static byte id = 28;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(UnconnectedPongPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteLongLE(fields.Time);
            encoder.WriteLongLE(fields.GUID);
            encoder.WriteMagic(fields.Magic);
            encoder.WriteRakString(fields.MOTD);
            encoder.SendPacket(id);
        }
    }
}

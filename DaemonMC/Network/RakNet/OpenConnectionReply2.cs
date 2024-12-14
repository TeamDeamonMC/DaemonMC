namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply2Packet
    {
        public string Magic { get; set; }
        public long GUID { get; set; }
        public IPAddressInfo clientAddress { get; set; }
        public int Mtu { get; set; }
        public bool Encryption { get; set; }
    }

    public class OpenConnectionReply2
    {
        public static byte id = 8;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(OpenConnectionReply2Packet fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteMagic(fields.Magic);
            encoder.WriteLongLE(fields.GUID);
            encoder.WriteAddress(); //todo
            encoder.WriteShortBE((ushort)fields.Mtu);
            encoder.WriteByte(0); //todo
            encoder.SendPacket(id);
        }
    }
}

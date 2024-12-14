using System;

namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply1Packet
    {
        public string Magic { get; set; }
        public long GUID { get; set; }
        public bool Security { get; set; }
        public int Cookie { get; set; }
        public int Mtu { get; set; }
    }

    public class OpenConnectionReply1
    {
        public static byte id = 6;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(OpenConnectionReply1Packet fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteMagic(fields.Magic);
            encoder.WriteLongLE(fields.GUID);
            encoder.WriteByte(0);
            encoder.WriteShortBE((ushort)fields.Mtu);
            encoder.SendPacket(id);
        }
    }
}

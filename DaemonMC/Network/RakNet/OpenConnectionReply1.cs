namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply1 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionReply1;

        public string Magic { get; set; }
        public long GUID { get; set; }
        public bool Security { get; set; }
        public int Cookie { get; set; }
        public short Mtu { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadMagic();
            GUID = decoder.ReadLongLE();
            Security = decoder.ReadBool();
            //todo cookie
            Mtu = decoder.ReadShortBE();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteMagic(Magic);
            encoder.WriteLongLE(GUID);
            encoder.WriteBool(Security);
            encoder.WriteShortBE((ushort)Mtu);
        }
    }
}

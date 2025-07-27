namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply1 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionReply1;

        public byte[] Magic { get; set; }
        public long GUID { get; set; }
        public bool Security { get; set; }
        public int Cookie { get; set; }
        public int Mtu { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadBytes(16);
            GUID = decoder.ReadLongLE();
            Security = decoder.ReadBool();
            Mtu = decoder.ReadShortBE();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBytes(Magic, false);
            encoder.WriteLongLE(GUID);
            encoder.WriteBool(Security);
            encoder.WriteShortBE((ushort)Mtu);
        }
    }
}

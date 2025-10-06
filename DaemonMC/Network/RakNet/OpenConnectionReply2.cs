namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply2 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionReply2;

        public string Magic { get; set; }
        public long GUID { get; set; }
        public IPAddressInfo Address { get; set; }
        public int Mtu { get; set; }
        public bool Encryption { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadMagic();
            GUID = decoder.ReadLongLE();
            Address = decoder.ReadAddress();
            Mtu = decoder.ReadShortBE();
            Encryption = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteMagic(Magic);
            encoder.WriteLongLE(GUID);
            encoder.WriteAddress(Address);
            encoder.WriteShortBE((ushort)Mtu);
            encoder.WriteBool(Encryption);
        }
    }
}

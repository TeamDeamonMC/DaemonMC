namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply2 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionReply2;

        public byte[] Magic { get; set; }
        public long GUID { get; set; }
        public IPAddressInfo clientAddress { get; set; }
        public int Mtu { get; set; }
        public bool Encryption { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadBytes(16);
            GUID = decoder.ReadLongLE();
            clientAddress = decoder.ReadAddress();
            Mtu = decoder.ReadShortBE();
            Encryption = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBytes(Magic, false);
            encoder.WriteLongLE(GUID);
            encoder.WriteAddress(clientAddress.IPAddress.ToString());
            encoder.WriteShortBE((ushort)Mtu);
            encoder.WriteBool(Encryption);
        }
    }
}

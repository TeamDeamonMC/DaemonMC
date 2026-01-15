namespace DaemonMC.Network.RakNet
{
    public class ACK : Packet
    {
        public override int Id => (int) Info.RakNet.ACK;

        public List<ACKdata> ACKs { get; set; } = new List<ACKdata>();

        protected override void Decode(PacketDecoder decoder)
        {
            var ACKlist = new List<ACKdata>();
            var count = decoder.ReadShortBE();
            for (int i = 0; i < count; ++i)
            {
                var ACK = new ACKdata();
                ACK.singleSequence = decoder.ReadBool();
                if (ACK.singleSequence == true)
                {
                    ACK.sequenceNumber = decoder.ReadUInt24LE();
                }
                else
                {
                    ACK.firstSequenceNumber = decoder.ReadUInt24LE();
                    ACK.lastSequenceNumber = decoder.ReadUInt24LE();
                }
                ACKs.Add(ACK);
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteShortBE(1);
            encoder.WriteBool(true);
            encoder.WriteUInt24LE(ACKs[0].sequenceNumber);
        }
    }
}

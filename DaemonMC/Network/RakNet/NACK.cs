namespace DaemonMC.Network.RakNet
{
    public class NACK : Packet
    {
        public override int Id => (int) Info.RakNet.NACK;

        public List<ACKdata> NACKs { get; set; } = new List<ACKdata>();

        protected override void Decode(PacketDecoder decoder)
        {
            var NACKlist = new List<ACKdata>();
            var count = decoder.ReadSignedShort();
            for (int i = 0; i < count; ++i)
            {
                var NACK = new ACKdata();
                NACK.singleSequence = decoder.ReadBool();
                if (NACK.singleSequence == true)
                {
                    NACK.sequenceNumber = decoder.ReadUInt24LE();
                }
                else
                {
                    NACK.firstSequenceNumber = decoder.ReadUInt24LE();
                    NACK.lastSequenceNumber = decoder.ReadUInt24LE();
                }
                NACKlist.Add(NACK);
                NACKs = NACKlist;
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

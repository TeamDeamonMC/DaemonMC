using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class NACKPacket
    {
        public List<NACKdata> NACKs { get; set; }
    }

    public class NACKdata
    {
        public bool singleSequence { get; set; }
        public uint sequenceNumber { get; set; }
        public uint firstSequenceNumber { get; set; }
        public uint lastSequenceNumber { get; set; }
    }

    public class NACK
    {
        public static byte id = 160;
        public static void Decode(PacketDecoder decoder)
        {
            var NACKs = new List<NACKdata>();
            var count = decoder.ReadSignedShort();
            for (int i = 0; i < count; ++i)
            {
                var NACK = new NACKdata();
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
                NACKs.Add(NACK);
            }
            var packet = new NACKPacket
            {
                NACKs = NACKs
            };

            RakPacketProcessor.NACK(packet, decoder.clientEp);
        }

        public static void Encode(ACKPacket fields)
        {

        }
    }
}

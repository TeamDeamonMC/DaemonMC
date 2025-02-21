using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class ACKPacket
    {
        public List<ACKdata> ACKs { get; set; }
    }

    public class ACKdata
    {
        public bool singleSequence { get; set; }
        public uint sequenceNumber { get; set; }
        public uint firstSequenceNumber { get; set; }
        public uint lastSequenceNumber { get; set; }
    }

    public class ACK
    {
        public static byte id = 192;
        public static void Decode(PacketDecoder decoder)
        {
            var ACKs = new List<ACKdata>();
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
            var packet = new ACKPacket
            {
                ACKs = ACKs
            };

            RakPacketProcessor.ACK(packet, decoder.endpoint);
        }

        public static void Encode(ACKPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteShortBE(1);
            encoder.WriteBool(true);
            encoder.WriteUInt24LE(fields.ACKs[0].sequenceNumber);
            encoder.SendPacket(id);
        }
    }
}

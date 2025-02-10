namespace DaemonMC.Network.RakNet
{
    public class FragmentedPacket
    {
        public int TotalSize;
        public int ReceivedSize;
        public byte[][] Fragments;

        public FragmentedPacket(int totalSize, int fragmentCount)
        {
            TotalSize = totalSize;
            ReceivedSize = 0;
            Fragments = new byte[fragmentCount][];
        }
    }

    public class Reliability
    {
        public static uint reliableIndex = 0;
        public static Dictionary<short, FragmentedPacket> fragmentedPackets = new Dictionary<short, FragmentedPacket>();

        public static void ReliabilityHandler(PacketDecoder decoder, int recv)
        {
            uint sequence = decoder.ReadUInt24LE();
            uint reliableIndex = 0;
            uint sequenceIndex = 0;
            uint orderIndex = 0;
            byte orderChannel = 0;

            int compSize = 0;
            short compId = 0;
            int compIndex = 0;

            while (decoder.readOffset < recv)
            {
                var flags = decoder.ReadByte();
                var pLength = decoder.ReadShortBE();

                byte reliabilityType = (byte)((flags & 0b011100000) >> 5);
                bool isFragmented = (flags & 0b00010000) > 0;

                if (reliabilityType == 0)
                {
                    //nothing
                }
                else if (reliabilityType == 1)
                {
                    reliableIndex = decoder.ReadUInt24LE();
                    sequenceIndex = decoder.ReadUInt24LE();
                }
                else if (reliabilityType == 2)
                {
                    reliableIndex = decoder.ReadUInt24LE();
                }
                else if (reliabilityType == 3)
                {
                    reliableIndex = decoder.ReadUInt24LE();
                    sequenceIndex = decoder.ReadUInt24LE();
                    orderChannel = decoder.ReadByte();
                }
                else if (reliabilityType == 4)
                {
                    reliableIndex = decoder.ReadUInt24LE();
                    orderIndex = decoder.ReadUInt24LE();
                    orderChannel = decoder.ReadByte();
                }
                else if (reliabilityType == 5)
                {
                    //nothing
                }
                else if (reliabilityType == 6)
                {
                    reliableIndex = decoder.ReadUInt24LE();
                }
                else if (reliabilityType == 7)
                {
                    reliableIndex = decoder.ReadUInt24LE();
                    orderIndex = decoder.ReadUInt24LE();
                    orderChannel = decoder.ReadByte();
                }

                if (isFragmented)
                {
                    compSize = decoder.ReadIntBE();
                    compId = decoder.ReadShortBE();
                    compIndex = decoder.ReadIntBE();
                }

                int lengthInBytes = (pLength + 7) / 8;
                byte[] body = new byte[lengthInBytes];
                Array.Copy(decoder.buffer, decoder.readOffset, body, 0, lengthInBytes);
                decoder.readOffset += lengthInBytes;

                if (isFragmented)
                {
                    if (!fragmentedPackets.ContainsKey(compId))
                    {
                        fragmentedPackets[compId] = new FragmentedPacket(compSize, lengthInBytes);
                    }

                    var fragment = fragmentedPackets[compId];
                    fragment.Fragments[compIndex] = body;
                    fragment.ReceivedSize += body.Length;

                    if (compSize == compIndex+1)
                    {
                        byte[] fullPacket = ReassemblePacket(fragment);
                        decoder.packetBuffers.Add(fullPacket);
                        fragmentedPackets.Remove(compId);
                    }
                }
                else
                {
                    decoder.packetBuffers.Add(body);
                }
                //Console.WriteLine($"[Frame Set Packet] seq: {sequence} f: {flags} pL: {pLength} rtype: {reliabilityType} frag: {isFragmented} relIndx: {reliableIndex} seqIndxL: {sequenceIndex} ordIndx: {orderIndex} ordCh: {orderChannel} compSize: {compSize} compIndx: {compIndex} compId: {compId}");

                var ack = new ACKdata { sequenceNumber = sequence };

                var acks = new List<ACKdata>();
                acks.Add(ack);

                PacketEncoder encoder = PacketEncoderPool.Get(decoder.endpoint);
                var pk = new ACKPacket
                {
                    ACKs = acks,
                };
                ACK.Encode(pk, encoder);
            }
        }

        private static byte[] ReassemblePacket(FragmentedPacket fragment)
        {
            byte[] fullPacket = new byte[600000]; //todo better to know size of the packet
            int offset = 0;

            foreach (var part in fragment.Fragments)
            {
                if (part != null)
                {
                    Array.Copy(part, 0, fullPacket, offset, part.Length);
                    offset += part.Length;
                }
            }

            return fullPacket;
        }

        public static void ReliabilityHandler(
        PacketEncoder encoder,
        byte[] body,
        byte reliabilityType = 2,
        bool isFragmented = false,
        uint sequenceIndex = 0,
        uint orderIndex = 0,
        byte orderChannel = 0,
        int compSize = 0,
        ushort compId = 0,
        int compIndex = 0)
        {
            int maxPayloadSize = 1500; //idk
            isFragmented = body.Length > maxPayloadSize;
            int totalFragments = isFragmented ? (int)Math.Ceiling((double)body.Length / maxPayloadSize) : 1;

            compId = 0;
            compIndex = 0;

            for (int i = 0; i < totalFragments; i++)
            {
                int start = i * maxPayloadSize;
                int length = Math.Min(maxPayloadSize, body.Length - start);

                byte flags = (byte)((reliabilityType << 5) & 0b01110000);
                if (isFragmented)
                {
                    flags |= 0b00010000;
                }
                else
                {
                    flags |= 0x00;
                }

                encoder.WriteByte(128);
                encoder.WriteUInt24LE(RakSessionManager.getSession(encoder.clientEp).sequenceNumber);
                encoder.WriteByte(flags);
                encoder.WriteShortBE((ushort)(isFragmented ? length * 8 : body.Count() * 8));

                if (reliabilityType == 0) // Unreliable
                {
                    // nothing
                }
                else if (reliabilityType == 1) // Unreliable Sequenced
                {
                    encoder.WriteUInt24LE(reliableIndex);
                    encoder.WriteUInt24LE(sequenceIndex);
                }
                else if (reliabilityType == 2) // Reliable
                {
                    encoder.WriteUInt24LE(reliableIndex);
                    reliableIndex++;
                }
                else if (reliabilityType == 3) // Ordered
                {
                    encoder.WriteUInt24LE(reliableIndex);
                    encoder.WriteUInt24LE(orderIndex);
                    encoder.WriteByte(orderChannel);
                    reliableIndex++;
                    orderIndex++;
                }
                else if (reliabilityType == 4) // Reliable Ordered
                {
                    encoder.WriteUInt24LE(reliableIndex);
                    encoder.WriteUInt24LE(orderIndex);
                    encoder.WriteByte(orderChannel);
                }
                else if (reliabilityType == 5) // Reliable Sequenced
                {
                    // nothing
                }
                else if (reliabilityType == 6) // Unreliable, ACK
                {
                    encoder.WriteUInt24LE(reliableIndex);
                }
                else if (reliabilityType == 7) // Reliable, ACK
                {
                    encoder.WriteUInt24LE(reliableIndex);
                    encoder.WriteUInt24LE(orderIndex);
                    encoder.WriteByte(orderChannel);
                }

                if (isFragmented)
                {
                    encoder.WriteIntBE(totalFragments);
                    encoder.WriteShortBE(compId);
                    encoder.WriteIntBE(compIndex);

                    byte[] fragment = new byte[length];
                    Array.Copy(body, start, fragment, 0, length);

                    Array.Copy(body, start, encoder.byteStream, encoder.writeOffset, length);
                    encoder.writeOffset += length;
                    compIndex++;
                }
                else
                {
                    Array.Copy(body, 0, encoder.byteStream, encoder.writeOffset, body.Length);
                    encoder.writeOffset += body.Length;
                }

                encoder.SendPacket(128, false);
                encoder.byteStream = new byte[512000];
                encoder.writeOffset = 0;
            }
            PacketEncoderPool.Return(encoder);
        }
    }
}

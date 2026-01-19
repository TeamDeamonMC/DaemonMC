using DaemonMC.Utils.Text;
using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class FragmentedPacket
    {
        public int TotalSize;
        public int ReceivedSize;
        public int Count;
        public byte[][] Fragments;

        public FragmentedPacket(int fragmentCount, int totalSize)
        {
            TotalSize = totalSize;
            ReceivedSize = 0;
            Count = 0;
            Fragments = new byte[fragmentCount][];
        }
    }

    public enum ReliabilityType
    {
        unreliable,
        unreliableSequenced,
        reliable,
        reliableOrdered,
        reliableSequenced,
        unreliableACK,
        reliableACK,
        reliableOrderedACK
    }

    public class Reliability
    {
        public static uint reliableIndex = 0;
        public static Dictionary<IPAddress, FragmentedPacket> fragmentedPackets = new Dictionary<IPAddress, FragmentedPacket>();

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

                int lengthInBytes = (pLength + 7) >> 3;
                byte[] body = new byte[lengthInBytes];
                Array.Copy(decoder.buffer, decoder.readOffset, body, 0, lengthInBytes);
                decoder.readOffset += lengthInBytes;

                if (isFragmented)
                {
                    if (!fragmentedPackets.ContainsKey(decoder.clientEp.Address))
                    {
                        fragmentedPackets[decoder.clientEp.Address] = new FragmentedPacket(compSize, lengthInBytes);
                    }

                    var fragment = fragmentedPackets[decoder.clientEp.Address];
                    fragment.Fragments[compIndex] = body;
                    fragment.ReceivedSize += body.Length;
                    fragment.Count++;

                    if (compSize == fragment.Count)
                    {
                        byte[] fullPacket = ReassemblePacket(fragment);
                        decoder.packetBuffers.Add(fullPacket);
                        fragmentedPackets.Remove(decoder.clientEp.Address);
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

                PacketEncoder encoder = PacketEncoderPool.Get(decoder.clientEp);
                var pk = new ACK
                {
                    ACKs = acks,
                };
                pk.EncodePacket(encoder);
                Server.AckOut++;
            }
        }

        private static byte[] ReassemblePacket(FragmentedPacket fragment)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var part in fragment.Fragments)
                {
                    if (part != null)
                    {
                        stream.Write(part, 0, part.Length);
                    }
                }
                return stream.ToArray();
            }
        }

        public static void ReliabilityHandler(PacketEncoder encoder, byte[] body, ReliabilityType reliabilityType = ReliabilityType.reliable)
        {
            var session = RakSessionManager.getSession(encoder.clientEp);

            if (session == null)
            {
                PacketEncoderPool.Return(encoder);
                return;
            }

            lock (session.Sync)
            {

                int maxPayloadSize = session.MTU - 32;
                bool isFragmented = body.Length > maxPayloadSize;
                maxPayloadSize -= reliabilityType == ReliabilityType.reliable ? 6 : 3;
                maxPayloadSize -= isFragmented ? 10 : 0;
                int totalFragments = isFragmented ? (int)Math.Ceiling((double)body.Length / maxPayloadSize) : 1;

                int compIndex = 0;
                uint seqIndex = 0;
                byte orderChannel = 0;

                for (int i = 0; i < totalFragments; i++)
                {
                    session.sentPackets.TryAdd(RakSessionManager.getSession(encoder.clientEp).sequenceNumber, (body, false));

                    int start = i * maxPayloadSize;
                    int length = Math.Min(maxPayloadSize, body.Length - start);

                    byte flags = (byte)(((int)reliabilityType << 5) & 0b01110000);
                    if (isFragmented)
                    {
                        flags |= 0b00010000;
                    }
                    else
                    {
                        flags |= 0x00;
                    }

                    encoder.WriteByte((byte)(isFragmented ? 140 : 128));
                    encoder.WriteUInt24LE(session.sequenceNumber);
                    encoder.WriteByte(flags);
                    encoder.WriteShortBE((ushort)(isFragmented ? length * 8 : body.Count() * 8));

                    if (reliabilityType == ReliabilityType.unreliableSequenced || reliabilityType == ReliabilityType.reliableSequenced)
                    {
                        encoder.WriteUInt24LE(seqIndex);
                        seqIndex++;
                    }

                    if (reliabilityType == ReliabilityType.reliable || reliabilityType == ReliabilityType.reliableOrdered || reliabilityType == ReliabilityType.reliableSequenced || reliabilityType == ReliabilityType.reliableACK || reliabilityType == ReliabilityType.reliableOrderedACK)
                    {
                        encoder.WriteUInt24LE(reliableIndex);
                        reliableIndex++;
                    }

                    if (reliabilityType == ReliabilityType.unreliableSequenced || reliabilityType == ReliabilityType.reliableOrdered || reliabilityType == ReliabilityType.reliableSequenced || reliabilityType == ReliabilityType.reliableOrderedACK)
                    {
                        encoder.WriteUInt24LE(session.orderIndex);
                        encoder.WriteByte(orderChannel);
                    }

                    if (isFragmented)
                    {
                        encoder.WriteIntBE(totalFragments);
                        encoder.WriteShortBE(session.compId);
                        encoder.WriteIntBE(compIndex);

                        byte[] fragment = new byte[length];
                        Array.Copy(body, start, fragment, 0, length);

                        encoder.byteStream.Write(fragment, 0, length);
                        compIndex++;
                    }
                    else
                    {
                        encoder.byteStream.Write(body, 0, body.Length);
                    }
                    encoder.SendPacket(128, false);
                    encoder.byteStream.SetLength(0);
                    encoder.byteStream.Position = 0;
                }

                if (isFragmented)
                {
                    session.compId++;
                }

                if (reliabilityType == ReliabilityType.unreliableSequenced || reliabilityType == ReliabilityType.reliableOrdered || reliabilityType == ReliabilityType.reliableSequenced || reliabilityType == ReliabilityType.reliableOrderedACK)
                {
                    session.orderIndex++;
                }
            }

            PacketEncoderPool.Return(encoder);
        }

        internal static void ResendPacket(uint sequenceNumber, IPEndPoint clientEp)
        {
            var session = RakSessionManager.getSession(clientEp);
            var sentPackets = session.sentPackets;

            if (sentPackets.TryGetValue(sequenceNumber, out var data))
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                sentPackets.Remove(sequenceNumber);
                ReliabilityHandler(encoder, data.Item1);
                Log.debug($"[RakNet] Received NACK {sequenceNumber} from {clientEp.Address}. Resending... OK", ConsoleColor.DarkYellow);
                Server.Rsent++;
            }
            else
            {
                Log.debug($"[RakNet] Received NACK {sequenceNumber} from {clientEp.Address}. Resending... FAILED. Unexpected sequence number", ConsoleColor.DarkYellow);
            }
            // Log.debug($"[RakNet] Currently unacknowledged messages({sentPackets.Count}):[{string.Join(", ", sentPackets.Keys)}]", ConsoleColor.DarkYellow);
            Server.NackIn++;

            if (session.Nacks > 30)
            {
                var player = Server.GetPlayer(session.EntityID);
                player.Kick($"Unacknowledged packet limit exceeded");
                Log.warn($"{player.Username} (MTU:{session.MTU}) exceeded unacknowledged packet limit");
            }
        }
    }
}

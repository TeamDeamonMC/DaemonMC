using System.Net;
using System.Numerics;
using System.Text;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network
{
    public class PacketDecoder
    {
        public List<byte[]> packetBuffers = new List<byte[]>();
        public byte[] buffer;
        public int readOffset;
        public IPEndPoint endpoint;
        public Player player;

        public PacketDecoder(byte[] byteBuffer, IPEndPoint clientEp)
        {
            buffer = byteBuffer;
            readOffset = 0;
            endpoint = clientEp;
        }

        public void RakDecoder(PacketDecoder decoder, int recv)
        {
            Server.datGrIn++;
            var clientIp = decoder.endpoint.Address.ToString();
            var clientPort = decoder.endpoint.Port;

            var pkid = decoder.ReadByte();
            if (pkid <= 127 || pkid >= 141) { Log.debug($"[Server] <-- [{clientIp,-16}:{clientPort}] {(Info.RakNet)pkid}"); }


            if (pkid == UnconnectedPing.id)
            {
                UnconnectedPing.Decode(decoder);
            }
            else if (pkid == OpenConnectionRequest1.id)
            {
                OpenConnectionRequest1.Decode(decoder, recv);
            }
            else if (pkid == OpenConnectionRequest2.id)
            {
                OpenConnectionRequest2.Decode(decoder);
            }
            else if (pkid == ACK.id)
            {
                ACK.Decode(decoder);
            }
            else if (pkid == NACK.id)
            {
                NACK.Decode(decoder);
            }
            else
            {
                if (pkid >= 128 && pkid <= 141) // Frame Set Packet
                {
                    Reliability.ReliabilityHandler(decoder, recv);
                }
                else
                {
                    Log.error($"[Server] Unknown RakNet packet: {pkid}");
                    DataTypes.HexDump(buffer, recv);
                }
            }
            packetHandler(decoder);
            PacketDecoderPool.Return(decoder);
        }

        public void packetHandler(PacketDecoder decoderT)
        {
            foreach (byte[] buffer in packetBuffers)
            {
                PacketDecoder decoder = PacketDecoderPool.Get(buffer, decoderT.endpoint);
                var pkid = decoder.ReadByte();
                if (pkid != 254) { Log.debug($"[Server] <-- [{decoder.endpoint.Address,-16}:{decoder.endpoint.Port}] {(Info.RakNet)pkid}"); }
                if (pkid == ConnectionRequest.id)
                {
                    ConnectionRequest.Decode(decoder);
                }
                else if (pkid == NewIncomingConnection.id)
                {
                    NewIncomingConnection.Decode(decoder);
                }
                else if (pkid == ConnectedPing.id)
                {
                    ConnectedPing.Decode(decoder);
                }
                else if (pkid == RakDisconnect.id)
                {
                    RakDisconnect.Decode(decoder);
                }
                else
                {
                    if (pkid == 254)
                    {
                        BedrockPacketDecoder.BedrockDecoder(decoder);
                    }
                    else
                    {
                        Log.error($"[Server] Unknown RakNet packet2: {pkid}");
                    }
                }
                PacketDecoderPool.Return(decoder);
            }
        }

        public void Reset(byte[] buffer)
        {
            this.buffer = buffer;
            this.readOffset = 0;
            this.packetBuffers.Clear();
        }

        public bool ReadBool()
        {
            byte b = buffer[readOffset];
            readOffset += 1;
            return b == 1 ? true : false;
        }

        public int ReadInt()
        {
            int a = BitConverter.ToInt32(buffer, readOffset);
            readOffset += 4;
            return a;
        }

        public uint ReadUInt()
        {
            uint value = BitConverter.ToUInt32(buffer, readOffset);
            readOffset += 4;
            return value;
        }

        public float ReadFloat()
        {
            float a = BitConverter.ToInt32(buffer, readOffset);
            readOffset += 4;
            return a;
        }

        public int ReadIntBE()
        {
            Array.Reverse(buffer, readOffset, 4);
            int a = BitConverter.ToInt32(buffer, readOffset);
            readOffset += 4;
            return a;
        }

        public int ReadVarInt()
        {
            int value = 0;
            int size = 0;

            while (true)
            {
                byte currentByte = buffer[readOffset++];
                value |= (currentByte & 0x7F) << (size * 7);

                if ((currentByte & 0x80) == 0)
                {
                    break;
                }

                size++;
            }

            return value;
        }

        public int ReadSignedVarInt()
        {

            int rawVarInt = ReadVarInt();
            int value = (rawVarInt >> 1) ^ -(rawVarInt & 1);
            return value;
        }

        public short ReadShort()
        {
            short value = (short)((buffer[readOffset] << 8) | buffer[readOffset + 1]);
            readOffset += 2;
            return value;
        }

        public short ReadShortBE()
        {
            short value = (short)(buffer[readOffset + 1] | (buffer[readOffset] << 8));
            readOffset += 2;
            return value;
        }

        public ushort ReadUShort()
        {
            ushort value = (ushort)((buffer[readOffset] << 8) | buffer[readOffset + 1]);
            readOffset += 2;
            return value;
        }

        public byte ReadByte()
        {
            byte b = buffer[readOffset];
            readOffset += 1;
            return b;
        }

        public long ReadLong()
        {
            long value = BitConverter.ToInt64(buffer, readOffset);
            readOffset += 8;
            return value;
        }

        public long ReadLongLE()
        {
            Array.Reverse(buffer, readOffset, 8);
            long value = BitConverter.ToInt64(buffer, readOffset);
            readOffset += 8;
            return value;
        }

        public string ReadMagic()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; ++i)
            {
                sb.Append(buffer[readOffset + i].ToString("X2"));
            }
            readOffset += 16;
            return sb.ToString();
        }

        public string ReadRakString()
        {
            ushort length = BitConverter.ToUInt16(buffer, readOffset);
            readOffset += 2;
            string str = Encoding.UTF8.GetString(buffer, readOffset, length);
            readOffset += length;

            return str;
        }

        public string ReadString()
        {
            int length = ReadVarInt();
            string str = Encoding.UTF8.GetString(buffer, readOffset, length);
            readOffset += length;

            return str;
        }

        public int ReadMTU(int lenght)
        {
            int paddingSize = lenght - readOffset;

            int estimatedMTU = readOffset + paddingSize + 28;

            readOffset = (paddingSize + readOffset);

            return estimatedMTU;
        }

        public IPAddressInfo ReadAddress()
        {
            byte ipVersion = buffer[readOffset];
            readOffset++;

            IPAddressInfo ipAddressInfo = new IPAddressInfo();

            if (ipVersion == 4)
            {
                ipAddressInfo.IPAddress = new byte[4];
                Array.Copy(buffer, readOffset, ipAddressInfo.IPAddress, 0, 4);
                ipAddressInfo.Port = BitConverter.ToUInt16(buffer, readOffset + 4);
                readOffset += 6;
            }
            else if (ipVersion == 6)
            {
                ipAddressInfo.IPAddress = new byte[16];
                Array.Copy(buffer, readOffset + 4, ipAddressInfo.IPAddress, 0, 16);
                ipAddressInfo.Port = BitConverter.ToUInt16(buffer, readOffset + 2);
                readOffset += 32;
            }

            return ipAddressInfo;
        }

        public IPAddressInfo[] ReadInternalAddress(int count)
        {
            IPAddressInfo[] ipAddress = new IPAddressInfo[count];
            for (int i = 0; i < count; ++i)
            {
                byte ipVersion = buffer[readOffset];
                readOffset++;

                IPAddressInfo ipAddressInfo = new IPAddressInfo();

                if (ipVersion == 4)
                {
                    ipAddressInfo.IPAddress = new byte[4];
                    Array.Copy(buffer, readOffset, ipAddressInfo.IPAddress, 0, 4);
                    ipAddressInfo.Port = BitConverter.ToUInt16(buffer, readOffset + 4);
                    readOffset += 6;
                }
                else if (ipVersion == 6)
                {
                    ipAddressInfo.IPAddress = new byte[16];
                    Array.Copy(buffer, readOffset + 4, ipAddressInfo.IPAddress, 0, 16);
                    ipAddressInfo.Port = BitConverter.ToUInt16(buffer, readOffset + 2);
                    readOffset += 32;
                }
                ipAddress[i] = ipAddressInfo;
            }
            return ipAddress;
        }

        public uint ReadUInt24LE()
        {
            uint uint24leValue = (uint)(buffer[readOffset] | (buffer[readOffset + 1] << 8) | (buffer[readOffset + 2] << 16));
            readOffset += 3;
            return uint24leValue;
        }

        public long ReadVarLong()
        {
            long value = 0;
            int size = 0;

            while (true)
            {
                byte currentByte = buffer[readOffset++];
                value |= (long)(currentByte & 0x7F) << (size * 7);

                if ((currentByte & 0x80) == 0)
                {
                    break;
                }

                size++;
            }

            return value;
        }

        public Vector3 ReadVec3()
        {
            var value = new Vector3()
            {
                X = ReadFloat(),
                Y = ReadFloat(),
                Z = ReadFloat()
            };
            return value;
        }

        public Vector2 ReadVec2()
        {
            var value = new Vector2()
            {
                X = ReadFloat(),
                Y = ReadFloat()
            };
            return value;
        }

        public List<TEnum> Read<TEnum>() where TEnum : Enum
        {
            ulong value = (ulong)ReadVarLong();
            List<TEnum> result = new List<TEnum>();

            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                int index = Convert.ToInt32(enumValue);
                if ((value & (1UL << index)) != 0)
                {
                    result.Add(enumValue);
                }
            }
            return result;
        }
    }

    public static class PacketDecoderPool
    {
        public static Stack<PacketDecoder> pool = new Stack<PacketDecoder>();
        public static int cached = 0;
        public static int inUse = 0;

        public static PacketDecoder Get(byte[] buffer, IPEndPoint clientEp)
        {
            inUse++;
            if (pool.Count > 0)
            {
                PacketDecoder decoder = pool.Pop();
                decoder.Reset(buffer);
                decoder.endpoint = clientEp;
                return decoder;
            }
            else
            {
                cached++;
                return new PacketDecoder(buffer, clientEp);
            }
        }

        public static void Return(PacketDecoder decoder)
        {
            inUse--;
            pool.Push(decoder);
        }
    }
}

using System.Net;
using System.Numerics;
using System.Text;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;
using fNbt;

namespace DaemonMC.Network
{
    public class PacketEncoder
    {
        public IPEndPoint clientEp = null!;
        public int writeOffset = 0;
        public byte[] byteStream = new byte[25000];

        public static Dictionary<uint, byte[]> sentPackets = new Dictionary<uint, byte[]>();

        public PacketEncoder(IPEndPoint ep)
        {
            clientEp = ep;
            writeOffset = 0;
            byteStream = new byte[25000];
        }

        public void handlePacket(string type = "bedrock")
        {
            byte[] trimmedBuffer = new byte[writeOffset];
            Array.Copy(byteStream, trimmedBuffer, writeOffset);
            if (type == "bedrock")
            {
                var packetID = ToDataTypes.ReadVarInt(trimmedBuffer);

                Log.debug($"[Server] --> [{clientEp.Address,-16}:{clientEp.Port}] {(Info.Bedrock)packetID}");
                byte[] bedrockId = new byte[] { 254 };

                if (RakSessionManager.getSession(clientEp) != null)
                {
                    if (RakSessionManager.getSession(clientEp).initCompression)
                    {
                        bedrockId = new byte[] { 254, 255 };
                    }
                }

                byte[] lengthVarInt = ToDataTypes.GetVarint(writeOffset);

                byte[] header = new byte[bedrockId.Length + lengthVarInt.Length];
                Array.Copy(bedrockId, 0, header, 0, bedrockId.Length);
                Array.Copy(lengthVarInt, 0, header, bedrockId.Length, lengthVarInt.Length);

                byte[] newtrimmedBuffer = new byte[trimmedBuffer.Length + header.Length];
                Array.Copy(header, 0, newtrimmedBuffer, 0, header.Length);
                Array.Copy(trimmedBuffer, 0, newtrimmedBuffer, header.Length, trimmedBuffer.Length);

                writeOffset = 0;
                byteStream = new byte[25000];

                Reliability.ReliabilityHandler(this, newtrimmedBuffer);
                return;
            }

            Log.debug($"[Server] --> [{clientEp.Address,-16}:{clientEp.Port}] {(Info.RakNet)trimmedBuffer[0]}");

            writeOffset = 0;
            byteStream = new byte[25000];

            if (trimmedBuffer[0] == 3)
            {
                Reliability.ReliabilityHandler(this, trimmedBuffer, 0, false);
            }
            else
            {
                Reliability.ReliabilityHandler(this, trimmedBuffer);
            }
        }

        public void SendPacket(int pkid, bool pooled = true)
        {
            Server.datGrOut++;
            var clientIp = clientEp.Address.ToString();

            var clientPort = clientEp.Port;
            if (pkid <= 127 || pkid >= 141) { Log.debug($"[Server] --> [{clientIp,-16}:{clientPort}] {(Info.RakNet)pkid}"); };
            byte[] trimmedBuffer = new byte[writeOffset];
            Array.Copy(byteStream, trimmedBuffer, writeOffset);
            Server.Send(trimmedBuffer, clientEp);
            RakSessionManager.getSession(clientEp).sequenceNumber++;
            if (pooled) { PacketEncoderPool.Return(this); }
        }

        //FE FF 0C 05 00 00 00 04 74 65 73 74 00 00 00 00
        //FE - 254 Bedrock packet
        //FF - 255 no compression
        //05 - packet id

        public void Reset()
        {
            byteStream = new byte[25000];
            writeOffset = 0;
        }

        public void PacketId(Info.Bedrock id)
        {
            WriteVarInt((int) id);
        }

        public void WriteBool(bool value)
        {
            byteStream[writeOffset] = value == true ? (byte)1 : (byte)0;
            writeOffset += 1;
        }

        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, byteStream, writeOffset, 4);
            writeOffset += 4;
        }

        public void WriteIntBE(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            Array.Copy(bytes, 0, byteStream, writeOffset, 4);
            writeOffset += 4;
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, byteStream, writeOffset, 4);
            writeOffset += 4;
        }

        public void WriteVarInt(int value)
        {
            while ((value & -128) != 0)
            {
                byteStream[writeOffset++] = (byte)((value & 127) | 128);
                value >>= 7;
            }
            byteStream[writeOffset++] = (byte)(value & 127);
        }

        public void WriteSignedVarInt(int value)
        {
            uint zigzagEncoded = (uint)((value << 1) ^ (value >> 31));
            WriteVarInt((int)zigzagEncoded);
        }

        public void WriteShort(ushort value)
        {
            byteStream[writeOffset] = (byte)value;
            byteStream[writeOffset + 1] = (byte)(value >> 8);
            writeOffset += 2;
        }

        public void WriteShortBE(ushort value)
        {
            byteStream[writeOffset] = (byte)(value >> 8);
            byteStream[writeOffset + 1] = (byte)value;
            writeOffset += 2;
        }

        public void WriteByte(byte value)
        {
            byteStream[writeOffset] = value;
            writeOffset += 1;
        }

        private void WriteBytes(byte[] data)
        {
            Array.Copy(data, 0, byteStream, writeOffset, data.Length);
            writeOffset += data.Length;
        }

        public void WriteLongLE(long value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes);
            Array.Copy(valueBytes, 0, byteStream, writeOffset, 8);
            writeOffset += 8;
        }

        public void WriteLong(long value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            Array.Copy(valueBytes, 0, byteStream, writeOffset, 8);
            writeOffset += 8;
        }

        public void WriteMagic(string magic)
        {
            for (int i = 0; i < magic.Length; i += 2)
            {
                string byteString = magic.Substring(i, 2);
                byte b = byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber);
                byteStream[writeOffset++] = b;
            }
        }

        public void WriteRakString(string str)
        {
            ushort length = (ushort)str.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            Array.Reverse(lengthBytes);
            Array.Copy(lengthBytes, 0, byteStream, writeOffset, 2);
            writeOffset += 2;

            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            Array.Copy(strBytes, 0, byteStream, writeOffset, strBytes.Length);
            writeOffset += strBytes.Length;
        }

        public void WriteString(string str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            WriteVarInt(strBytes.Length);
            Array.Copy(strBytes, 0, byteStream, writeOffset, strBytes.Length);
            writeOffset += strBytes.Length;
        }

        public void WriteAddress(string ip = "127.0.0.1")
        {
            string[] ipParts = ip.Split('.');
            byte[] ipAddress = new byte[] { byte.Parse(ipParts[0]), byte.Parse(ipParts[1]), byte.Parse(ipParts[2]), byte.Parse(ipParts[3]) };
            ushort port = 19132;

            byteStream[writeOffset] = 4;
            writeOffset++;

            Array.Copy(ipAddress, 0, byteStream, writeOffset, ipAddress.Length);
            writeOffset += ipAddress.Length;

            byte[] portBytes = BitConverter.GetBytes(port);
            Array.Reverse(portBytes);
            Array.Copy(portBytes, 0, byteStream, writeOffset, portBytes.Length);
            writeOffset += portBytes.Length;
        }

        public void WriteUInt24LE(uint value)
        {
            byteStream[writeOffset] = (byte)(value & 0xFF);
            byteStream[writeOffset + 1] = (byte)((value >> 8) & 0xFF);
            byteStream[writeOffset + 2] = (byte)((value >> 16) & 0xFF);
            writeOffset += 3;
        }

        public void WriteVarLong(ulong value)
        {
            while ((value & ~0x7FUL) != 0)
            {
                byteStream[writeOffset++] = (byte)((value & 0x7FUL) | 0x80UL);
                value >>= 7;
            }
            byteStream[writeOffset++] = (byte)(value & 0x7FUL);
        }

        public void WriteSignedVarLong(long value)
        {
            ulong zigzagEncoded = (ulong)((value << 1) ^ (value >> 63));
            WriteVarLong(zigzagEncoded);
        }

        public void WriteCompoundTag(NbtCompound compoundTag)
        {
            NbtFile file = new NbtFile(compoundTag);

            file.BigEndian = false;
            file.UseVarInt = true;

            byte[] serializedTag = file.SaveToBuffer(NbtCompression.None);

            Array.Copy(serializedTag, 0, byteStream, writeOffset, serializedTag.Length);

            writeOffset += serializedTag.Length;
        }

        public void WriteUUID(Guid uuid)
        {
            byte[] uuidBytes = uuid.ToByteArray();
            byte[] mostSignificantBits = uuidBytes.Take(8).Reverse().ToArray();
            byte[] leastSignificantBits = uuidBytes.Skip(8).Take(8).Reverse().ToArray();

            WriteBytes(mostSignificantBits);
            WriteBytes(leastSignificantBits);
        }

        public void WriteVec3(Vector3 vec)
        {
            WriteFloat(vec.X);
            WriteFloat(vec.Y);
            WriteFloat(vec.Z);
        }

        public void WriteVec2(Vector2 vec)
        {
            WriteFloat(vec.X);
            WriteFloat(vec.Y);
        }
    }

    public class PacketEncoderPool
    {
        public static Stack<PacketEncoder> pool = new Stack<PacketEncoder>();
        public static int cached = 0;
        public static int inUse = 0;

        public static PacketEncoder Get(IPEndPoint ep)
        {
            inUse++;
            if (pool.Count > 0)
            {
                var encoder = pool.Pop();
                encoder.clientEp = ep;
                return encoder;
            }

            cached++;
            return new PacketEncoder(ep);
        }

        public static void Return(PacketEncoder encoder)
        {
            inUse--;
            encoder.Reset();
            pool.Push(encoder);
        }
    }
}

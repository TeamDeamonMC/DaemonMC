using System.Net;
using System.Numerics;
using System.Text;
using DaemonMC.Blocks;
using DaemonMC.Items;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network
{
    public class PacketDecoder
    {
        public List<byte[]> packetBuffers = new List<byte[]>();
        public byte[] buffer;
        public int readOffset;
        public IPEndPoint clientEp;
        public int protocolVersion = 0;
        public Player player;

        public PacketDecoder(byte[] byteBuffer, IPEndPoint ep)
        {
            buffer = byteBuffer;
            readOffset = 0;
            clientEp = ep;
            protocolVersion = RakSessionManager.getSession(ep).protocolVersion;
        }

        public void RakDecoder(PacketDecoder decoder, int recv)
        {
            Server.DatGrIn++;

            var pkid = decoder.ReadByte();
            if (pkid <= 127 || pkid >= 141) { Log.packetIn(decoder.clientEp, (Info.RakNet)pkid); }


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
                    ToDataTypes.HexDump(buffer, recv);
                }
            }
            if (decoder.readOffset < recv)
            {
                Log.warn($"{recv - decoder.readOffset} bytes left while reading {(Info.RakNet)pkid}");
            }
            packetHandler(decoder);
            PacketDecoderPool.Return(decoder);
        }

        public void packetHandler(PacketDecoder decoderT)
        {
            foreach (byte[] buffer in packetBuffers)
            {
                PacketDecoder decoder = PacketDecoderPool.Get(buffer, decoderT.clientEp);
                var pkid = decoder.ReadByte();
                if (pkid != 254) { Log.packetIn(decoder.clientEp, (Info.RakNet)pkid); }
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
                else if (pkid == (byte)Info.RakNet.Disconnect)
                {
                    new RakDisconnect().Decode(decoder);
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
                if (decoder.readOffset < decoder.buffer.Length && pkid != 254)
                {
                    Log.warn($"{decoder.buffer.Length - decoder.readOffset} bytes left while reading {(Info.RakNet)pkid}");
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

        public float ReadFloat()
        {
            float a = BitConverter.ToSingle(buffer, readOffset);
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

        public short ReadSignedShort()
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

        public ushort ReadShort()
        {
            ushort value = (ushort)((buffer[readOffset] << 8) | buffer[readOffset + 1]);
            readOffset += 2;
            return (ushort)((value >> 8) | (value << 8));
        }

        public byte ReadByte()
        {
            byte b = buffer[readOffset];
            readOffset += 1;
            return b;
        }

        public void ReadBytes(byte[] data)
        {
            Array.Copy(buffer, readOffset, data, 0, data.Length);
            readOffset += data.Length;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];
            Array.Copy(buffer, readOffset, result, 0, count);
            readOffset += count;

            return result;
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
            if (length < 0 || readOffset + length > buffer.Length)
            {
                throw new Exception($"Invalid string lenght {length}");
            }
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

        public IPAddressInfo[] ReadInternalAddress()
        {
            List<IPAddressInfo> internalAddresses = new List<IPAddressInfo>();

            while (buffer.Length - readOffset > 16)
            {
                byte ipVersion = buffer[readOffset++];
                IPAddressInfo ipAddressInfo = new IPAddressInfo();

                if (ipVersion == 4)
                {
                    if (buffer.Length - readOffset < 6) break;
                    ipAddressInfo.IPAddress = new byte[4];
                    Array.Copy(buffer, readOffset, ipAddressInfo.IPAddress, 0, 4);
                    ipAddressInfo.Port = BitConverter.ToUInt16(buffer, readOffset + 4);
                    readOffset += 6;
                }
                else if (ipVersion == 6)
                {
                    if (buffer.Length - readOffset < 31) break;
                    ipAddressInfo.IPAddress = new byte[16];
                    ipAddressInfo.Port = BitConverter.ToUInt16(buffer, readOffset + 2);
                    Array.Copy(buffer, readOffset + 4, ipAddressInfo.IPAddress, 0, 16);
                    readOffset += 32;
                }
                else
                {
                    break;
                }

                internalAddresses.Add(ipAddressInfo);
            }

            return internalAddresses.ToArray();
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

        public Guid ReadUUID()
        {
            byte[] mostSignificantBits = new byte[8];
            byte[] leastSignificantBits = new byte[8];

            ReadBytes(mostSignificantBits);
            ReadBytes(leastSignificantBits);

            mostSignificantBits = mostSignificantBits.Reverse().ToArray();
            leastSignificantBits = leastSignificantBits.Reverse().ToArray();

            byte[] uuidBytes = mostSignificantBits.Concat(leastSignificantBits).ToArray();
            return new Guid(uuidBytes);
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

        public List<string> ReadPackNames()
        {
            List<string> packs = new List<string>();
            ushort packCount = ReadShort();
            for (int i = 0; i < packCount; i++)
            {
                packs.Add(ReadString());
            }
            return packs;
        }

        public Skin ReadSkin()
        {
            Skin skin = new Skin();

            skin.SkinId = ReadString();
            skin.PlayFabId = ReadString();
            skin.SkinResourcePatch = ReadString();
            skin.SkinImageWidth = ReadInt();
            skin.SkinImageHeight = ReadInt();
            int skinDataLength = ReadVarInt();
            skin.SkinData = ReadBytes(skinDataLength);

            int animatedDataCount = ReadInt();
            skin.AnimatedImageData = new List<AnimatedImageData>();

            for (int i = 0; i < animatedDataCount; i++)
            {
                AnimatedImageData animation = new AnimatedImageData();
                animation.ImageWidth = ReadInt();
                animation.ImageHeight = ReadInt();
                int imageDataLength = ReadVarInt();
                animation.Image = Convert.ToBase64String(ReadBytes(imageDataLength));
                animation.Type = ReadInt();
                animation.Frames = ReadFloat();
                animation.AnimationExpression = ReadInt();

                skin.AnimatedImageData.Add(animation);
            }

            skin.Cape = new Cape();
            skin.Cape.CapeImageWidth = ReadInt();
            skin.Cape.CapeImageHeight = ReadInt();
            int capeDataLength = ReadVarInt();
            skin.Cape.CapeData = ReadBytes(capeDataLength);
            skin.SkinGeometryData = ReadString();
            skin.SkinGeometryDataEngineVersion = ReadString();
            skin.SkinAnimationData = ReadString();
            skin.Cape.CapeId = ReadString();
            ReadString();
            skin.ArmSize = ReadString();
            skin.SkinColor = ReadString();

            int personaPieceCount = ReadInt();
            skin.PersonaPieces = new List<PersonaPiece>();

            for (int i = 0; i < personaPieceCount; i++)
            {
                PersonaPiece part = new PersonaPiece();
                part.PieceId = ReadString();
                part.PieceType = ReadString();
                part.PackId = ReadString();
                part.IsDefault = ReadBool();
                part.ProductId = ReadString();

                skin.PersonaPieces.Add(part);
            }

            int pieceTintCount = ReadInt();
            skin.PieceTintColors = new List<PieceTintColor>();

            for (int i = 0; i < pieceTintCount; i++)
            {
                PieceTintColor part = new PieceTintColor();
                part.PieceType = ReadString();
                int colorCount = ReadInt();
                part.Colors = new List<string>();

                for (int j = 0; j < colorCount; j++)
                {
                    part.Colors.Add(ReadString());
                }

                skin.PieceTintColors.Add(part);
            }

            skin.PremiumSkin = ReadBool();
            skin.PersonaSkin = ReadBool();
            skin.CapeOnClassicSkin = ReadBool();
            ReadBool(); // is primary user
            skin.OverrideSkin = ReadBool();

            return skin;
        }

        public List<Guid> ReadEmotes()
        {
            var EmoteIds = new List<Guid>();
            var size = ReadVarInt();
            for (int v = 0; v < size; v++)
            {
                EmoteIds.Add(ReadUUID());
            }
            return EmoteIds;
        }

        public Item ReadItem()
        {
            var id = ReadSignedVarInt();
            if (id != 0)
            {
                if (ItemPalette.items.TryGetValue((short)id, out Item value))
                {
                    Item item = value.Clone();
                    item.Count = ReadShort();
                    item.Aux = ReadVarInt();
                    ReadBool();//??? todo
                    ReadSignedVarInt();//block runtime id
                    ReadString();//nbt data
                    return value;
                }
            }
            return new Items.VanillaItems.Air();
        }

        public T? ReadOptional<T>(Func<T> readFunction)
        {
            return ReadBool() ? readFunction() : default;
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
        public static Stack<PacketDecoder> Pool = new Stack<PacketDecoder>();
        public static int Cached = 0;
        public static int InUse = 0;

        public static PacketDecoder Get(byte[] buffer, IPEndPoint clientEp)
        {
            InUse++;
            if (Pool.Count > 0)
            {
                PacketDecoder decoder = Pool.Pop();
                decoder.Reset(buffer);
                decoder.clientEp = clientEp;
                decoder.protocolVersion = RakSessionManager.getSession(clientEp).protocolVersion;
                return decoder;
            }
            else
            {
                Cached++;
                return new PacketDecoder(buffer, clientEp);
            }
        }

        public static void Return(PacketDecoder decoder)
        {
            InUse--;
            Pool.Push(decoder);
        }
    }
}

using System;
using System.Net;
using System.Numerics;
using System.Text;
using DaemonMC.Items;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network
{
    public class PacketDecoder
    {
        public readonly object Sync = new object();
        public List<byte[]> packetBuffers = new List<byte[]>();
        public MemoryStream byteStream;
        public IPEndPoint clientEp;
        public int protocolVersion = 0;
        public Player player;

        public PacketDecoder(byte[] byteBuffer, IPEndPoint ep)
        {
            clientEp = ep;
            byteStream = new MemoryStream();
            protocolVersion = RakSessionManager.getSession(ep).protocolVersion;
        }

        public void RakDecoder(PacketDecoder decoder, int recv)
        {
            Server.DatGrIn++;

            var pkid = decoder.ReadByte();
            if (pkid <= 127 || pkid >= 141) { Log.packetIn(decoder.clientEp, (Info.RakNet)pkid); }

            switch ((Info.RakNet)pkid)
            {
                case Info.RakNet.UnconnectedPing:
                    new UnconnectedPing().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.UnconnectedPong:
                    new UnconnectedPong().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.OpenConnectionRequest1:
                    new OpenConnectionRequest1().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.OpenConnectionReply1:
                    new OpenConnectionReply1().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.OpenConnectionRequest2:
                    new OpenConnectionRequest2().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.OpenConnectionReply2:
                    new OpenConnectionReply2().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.ACK:
                    new ACK().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                case Info.RakNet.NACK:
                    new NACK().DecodePacket(decoder, PacketHandler.Raknet);
                    break;
                default:
                    if (pkid >= 128 && pkid <= 141)
                    {
                        Reliability.ReliabilityHandler(decoder, recv);
                    }
                    else
                    {
                        Log.error($"[Server] Unknown RakNet packet: {pkid}");
                        ToDataTypes.HexDump(decoder.byteStream.ToArray(), recv);
                    }
                    break;
            }

            if (decoder.byteStream.Position < recv)
            {
                Log.warn($"{recv - decoder.byteStream.Position} bytes left while reading {(Info.RakNet)pkid}");
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
                Log.packetIn(decoder.clientEp, (Info.RakNet)pkid);

                switch ((Info.RakNet)pkid)
                {
                    case Info.RakNet.ConnectionRequest:
                        new ConnectionRequest().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    case Info.RakNet.ConnectionRequestAccepted:
                        new ConnectionRequestAccepted().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    case Info.RakNet.NewIncomingConnection:
                        new NewIncomingConnection().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    case Info.RakNet.ConnectedPing:
                        new ConnectedPing().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    case Info.RakNet.ConnectedPong:
                        new ConnectedPong().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    case Info.RakNet.Disconnect:
                        new RakDisconnect().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    case Info.RakNet.GamePacket:
                        new GamePacket().DecodePacket(decoder, PacketHandler.Raknet);
                        break;
                    default:
                        Log.error($"[Server] Unknown RakNet packet: {pkid}");
                        break;
                }
                if (decoder.byteStream.Position < decoder.byteStream.Length && pkid != 254)
                {
                    Log.warn($"{decoder.byteStream.Length - decoder.byteStream.Position} bytes left while reading {(Info.RakNet)pkid}");
                }
                PacketDecoderPool.Return(decoder);
            }
        }

        public void Reset(byte[] buffer)
        {
            byteStream.SetLength(0);
            byteStream.Position = 0;
            this.packetBuffers.Clear();
        }

        public bool ReadBool()
        {
            int b = byteStream.ReadByte();
            return b == 1 ? true : false;
        }

        public int ReadInt()
        {
            byte[] bytes = new byte[4];
            byteStream.Read(bytes, 0, 4);
            return BitConverter.ToInt32(bytes, 0);
        }

        public float ReadFloat()
        {
            byte[] bytes = new byte[4];
            byteStream.Read(bytes, 0, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        public int ReadIntBE()
        {
            byte[] bytes = new byte[4];
            byteStream.Read(bytes, 0, 4);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public int ReadVarInt()
        {
            int value = 0;
            int size = 0;

            while (true)
            {
                int read = byteStream.ReadByte();

                byte currentByte = (byte)read;

                value |= (currentByte & 0x7F) << (size * 7);

                if ((currentByte & 0x80) == 0)
                    break;

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
            short value = (short)((byteStream.ReadByte() << 8) | byteStream.ReadByte());
            return value;
        }

        public short ReadShortBE()
        {
            byte[] bytes = new byte[4];
            byteStream.Read(bytes, 0, 4);
            short value = (short)(bytes[1] | (bytes[0] << 8));
            return value;
        }

        public ushort ReadShort()
        {
            short value = (short)((byteStream.ReadByte() << 8) | byteStream.ReadByte());
            return (ushort)((value >> 8) | (value << 8));
        }

        public byte ReadByte()
        {
            return (byte)byteStream.ReadByte();
        }

        public void ReadBytes(byte[] data)
        {
            byteStream.Read(data, 0, data.Length);
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];
            byteStream.Read(result, 0, count);

            return result;
        }

        public byte[] ReadBytes()
        {
            int length = ReadVarInt();

            byte[] result = new byte[length];
            byteStream.Read(result, 0, length);

            return result;
        }

        public long ReadLong()
        {
            byte[] bytes = new byte[8];
            byteStream.Read(bytes, 0, 8);
            return BitConverter.ToInt64(bytes, 0);
        }

        public long ReadLongLE()
        {
            byte[] bytes = new byte[8];
            byteStream.Read(bytes, 0, 8);
            Array.Reverse(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }

        public string ReadMagic()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 16; ++i)
            {
                sb.Append(byteStream.ReadByte().ToString("X2"));
            }
            return sb.ToString();
        }

        public string ReadRakString()
        {
            short length = ReadShortBE();
            byte[] bytes = new byte[length];
            byteStream.Read(bytes, 0, length);
            string str = Encoding.UTF8.GetString(bytes, 0, length);

            return str;
        }

        public string ReadString()
        {
            int length = ReadVarInt();
            byte[] bytes = new byte[length];
            byteStream.Read(bytes, 0, length);
            string str = Encoding.UTF8.GetString(bytes, 0, length);

            return str;
        }

        public List<string> ReadStringList()
        {
            List<string> list = new List<string>();
            int count = ReadVarInt();
            for (int i = 0; i < count; i++)
            {
                list.Add(ReadString());
            }
            return list;
        }

        public short ReadMTU(int lenght)
        {
            long paddingSize = lenght - byteStream.Position;

            short estimatedMTU = (short)(byteStream.Position + paddingSize + 28);

            byteStream.Position = (paddingSize + byteStream.Position);

            return estimatedMTU;
        }

        public IPAddressInfo ReadAddress()
        {
            int version = byteStream.ReadByte();

            IPAddressInfo ipAddressInfo = new IPAddressInfo();

            if (version == 4)
            {
                ipAddressInfo.IPAddress = new byte[4];
                byteStream.Read(ipAddressInfo.IPAddress, 0, 4);

                ipAddressInfo.Port = ReadShort();
            }
            else if (version == 6)
            {
                ReadShort();
                ipAddressInfo.Port = ReadShort();
                ReadInt();

                ipAddressInfo.IPAddress = new byte[16];
                byteStream.Read(ipAddressInfo.IPAddress, 0, 16);

                ReadInt();
            }

            return ipAddressInfo;
        }

        public IPAddressInfo[] ReadInternalAddress()
        {
            List<IPAddressInfo> internalAddresses = new List<IPAddressInfo>();

            for (int i = 0; i < 20; i++)
            {
                long remaining = byteStream.Length - byteStream.Position;
                if (remaining <= 0) break;

                int version = byteStream.ReadByte();
                if (version == -1) break;

                byteStream.Position--;

                bool valid = (version == 4 && remaining > 22) || (version == 6 && remaining > 44);

                if (valid)
                {
                    internalAddresses.Add(ReadAddress());
                }
                else
                {
                    Log.warn($"Unknown IP version {version}");
                    break;
                }
            }

            return internalAddresses.ToArray();
        }

        public uint ReadUInt24LE()
        {
            byte[] bytes = new byte[3];
            byteStream.Read(bytes, 0, 3);
            uint uint24leValue = (uint)(bytes[0] | (bytes[1] << 8) | (bytes[2] << 16));
            return uint24leValue;
        }

        public long ReadVarLong()
        {
            long value = 0;
            int size = 0;

            while (true)
            {
                int currentByte = byteStream.ReadByte();
                value |= (long)(currentByte & 0x7F) << (size * 7);

                if ((currentByte & 0x80) == 0)
                {
                    break;
                }

                size++;
            }

            return value;
        }

        public long ReadSignedVarLong()
        {

            long rawVarLong = ReadVarLong();
            long value = (rawVarLong >> 1) ^ -(rawVarLong & 1);
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

        public AttributesValues ReadAttributes()
        {
            var values = new AttributesValues();
            values.MovementSpeed = ReadFloat();
            values.UnderwaterMovementSpeed = ReadFloat();
            values.LavaMovementSpeed = ReadFloat();
            values.JumpStrength = ReadFloat();
            values.Health = ReadFloat();
            values.Hunger = ReadFloat();
            return values;
        }

        public List<Actions> ReadActions()
        {
            var actions = new List<Actions>();
            var count = ReadVarInt();
            for (int i = 0; i < count; i++)
            {
                var action = new Actions();
                action.ActionsType = ReadByte();
                action.Amount = ReadByte();
                action.Source = ReadSlotInfo();
                action.Destination = ReadSlotInfo();
            }
            return actions;
        }

        public ItemStackRequestSlotInfo ReadSlotInfo()
        {
            var slotInfo = new ItemStackRequestSlotInfo();
            slotInfo.ContainerName = ReadContainerName();
            slotInfo.Slot = ReadByte();
            slotInfo.NetIdVariant = ReadVarInt();
            return slotInfo;
        }

        public FullContainerName ReadContainerName()
        {
            var containerName = new FullContainerName();
            containerName.ContainerName = ReadByte();
            containerName.DynamicId = ReadOptional(ReadSignedVarInt);
            return containerName;
        }

        public PlayerBlockAction ReadBlockActions()
        {
            var action = new PlayerBlockAction();
            var actionCount = ReadSignedVarInt();
            for (int i = 0; i < actionCount; i++)
            {
                action.ActionType = (PlayerActionType)ReadVarInt();
                switch (action.ActionType)
                {
                    case PlayerActionType.PredictDestroyBlock:
                    case PlayerActionType.StartDestroyBlock:
                    case PlayerActionType.AbortDestroyBlock:
                    case PlayerActionType.CrackBlock:
                    case PlayerActionType.ContinueDestroyBlock:
                        action.X = ReadSignedVarInt();
                        action.Y = ReadSignedVarInt();
                        action.Z = ReadSignedVarInt();
                        action.Facing = ReadVarInt();
                        break;
                    default:
                        break;
                }
            }
            return action;
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

        public static PacketDecoder Get(byte[] buffer, IPEndPoint clientEp)
        {
            if (Pool.Count > 0)
            {
                var session = RakSessionManager.getSession(clientEp);
                PacketDecoder decoder = Pool.Pop();
                decoder.Reset(buffer);
                decoder.clientEp = clientEp;
                decoder.player = Server.OnlinePlayers.ContainsKey(session.EntityID) ? Server.GetPlayer(session.EntityID) : null;
                decoder.protocolVersion = session.protocolVersion;
                return decoder;
            }
            else
            {
                return new PacketDecoder(buffer, clientEp);
            }
        }

        public static void Return(PacketDecoder decoder)
        {
            Pool.Push(decoder);
        }
    }
}

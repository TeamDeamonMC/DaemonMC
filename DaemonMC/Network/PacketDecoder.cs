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
                        ToDataTypes.HexDump(decoder.buffer, recv);
                    }
                    break;
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

        public byte[] ReadBytes()
        {
            int length = ReadVarInt();
            byte[] result = new byte[length];
            Array.Copy(buffer, readOffset, result, 0, length);
            readOffset += length;

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
            short length = ReadShortBE();
            if (length < 0 || readOffset + length > buffer.Length)
            {
                throw new Exception($"Invalid rakstring lenght {length}");
            }
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
            int paddingSize = lenght - readOffset;

            short estimatedMTU = (short)(readOffset + paddingSize + 28);

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
                readOffset += 4;
                ipAddressInfo.Port = ReadShort();
            }
            else if (ipVersion == 6)
            {
                ReadShort(); //address family
                ipAddressInfo.Port = ReadShort();
                ReadInt(); //idk
                ipAddressInfo.IPAddress = new byte[16];
                Array.Copy(buffer, readOffset, ipAddressInfo.IPAddress, 0, 16);
                readOffset += 16;
                ReadInt(); //also idk
            }

            return ipAddressInfo;
        }

        public IPAddressInfo[] ReadInternalAddress()
        {
            List<IPAddressInfo> internalAddresses = new List<IPAddressInfo>();

            for (int i = 0; i < 20; i++)
            {
                byte ipVersion = buffer[readOffset];
                IPAddressInfo ipAddressInfo = new IPAddressInfo();

                if ((ipVersion == 4 && buffer.Length - readOffset > 16 + 6) || (ipVersion == 6 && buffer.Length - readOffset > 16 + 28))
                {
                    internalAddresses.Add(ReadAddress());
                }
                else
                {
                    Log.warn($"Unknown IP version {ipVersion}");
                    break;
                }
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

        public Vector3 ReadBlockNetPos()
        {
            var value = new Vector3();
            value.X = ReadSignedVarInt();
            value.Y = protocolVersion >= Info.v1_26_10 ? ReadSignedVarInt() : ReadVarInt(); //before 1.26.10 Y was unsigned varint
            value.Z = ReadSignedVarInt();
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
            int packCount = protocolVersion >= Info.v1_26_40 ? ReadVarInt() : ReadShort();
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
            if (protocolVersion < Info.v1_26_60)
            {
                skin.PlayFabId = ReadString();
            }
            skin.SkinResourcePatch = ReadString();
            skin.SkinImageWidth = ReadInt();
            skin.SkinImageHeight = ReadInt();
            int skinDataLength = ReadVarInt();
            skin.SkinData = ReadBytes(skinDataLength);

            int animatedDataCount = protocolVersion >= Info.v1_26_40 ? ReadVarInt() : ReadInt();
            skin.AnimatedImageData = new List<AnimatedImageData>();

            for (int i = 0; i < animatedDataCount; i++)
            {
                AnimatedImageData animation = new AnimatedImageData();
                animation.ImageWidth = ReadInt();
                animation.ImageHeight = ReadInt();
                int imageDataLength = ReadVarInt();
                animation.Image = Convert.ToBase64String(ReadBytes(imageDataLength));
                animation.Type = protocolVersion >= Info.v1_26_40 ? ReadVarInt() : ReadInt();
                animation.Frames = ReadFloat();
                animation.AnimationExpression = protocolVersion >= Info.v1_26_40 ? ReadVarInt() : ReadInt();

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
            if (protocolVersion >= Info.v1_26_40)
            {
                skin.ArmSize = ReadByte(); //0 slim, 1 wide
                skin.SkinColor = ReadInt(); //ARGB
            }
            else
            {
                skin.ArmSize = (byte)(ReadString() == "slim" ? 0 : 1);
                skin.SkinColor = Skin.ColorToInt(ReadString());
            }

            int personaPieceCount = protocolVersion >= Info.v1_26_40 ? ReadVarInt() : ReadInt();
            skin.PersonaPieces = new List<PersonaPiece>();

            for (int i = 0; i < personaPieceCount; i++)
            {
                PersonaPiece part = new PersonaPiece();
                part.PieceId = ReadString();
                part.PieceType = protocolVersion >= Info.v1_26_40 ? PersonaPieceTypesExtensions.FromId(ReadInt()).ToString() : ReadString();
                part.PackId = protocolVersion >= Info.v1_26_40 ? ReadUUID().ToString() : ReadString();
                part.IsDefault = ReadBool();
                part.ProductId = ReadString();

                skin.PersonaPieces.Add(part);
            }

            int pieceTintCount = protocolVersion >= Info.v1_26_40 ? ReadVarInt() : ReadInt();
            skin.PieceTintColors = new List<PieceTintColor>();

            for (int i = 0; i < pieceTintCount; i++)
            {
                PieceTintColor part = new PieceTintColor();
                part.PieceType = ReadString();
                part.Colors = new List<string>();

                if (protocolVersion >= Info.v1_26_40)
                {
                    for (int j = 0; j < 4; j++) //always 4 ARGB colors
                    {
                        part.Colors.Add(Skin.IntToColor(ReadInt()));
                    }
                }
                else
                {
                    int colorCount = ReadInt();
                    for (int j = 0; j < colorCount; j++)
                    {
                        part.Colors.Add(ReadString());
                    }
                }

                skin.PieceTintColors.Add(part);
            }

            skin.PremiumSkin = ReadBool();
            skin.PersonaSkin = ReadBool();
            skin.CapeOnClassicSkin = ReadBool();
            ReadBool(); // is primary user
            skin.OverrideSkin = ReadBool();
            if (protocolVersion >= Info.v1_26_40)
            {
                skin.trustedSkinFlag = ReadString();
                skin.profileHash = ReadString();
            }

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

        public Item ReadNetItem()
        {
            return ReadItem(true);
        }
        public Item ReadItem(bool network = false)
        {
            var id = 0;
            if (network)
            {
                id = (short)ReadShort();
            }
            else
            {
                id = ReadSignedVarInt();
            }
            if (id != 0 || network)
            {
                Item item = ItemPalette.items.GetValueOrDefault((short)id)?.Clone();

                if (item == null)
                {
                    item = new Items.VanillaItems.Air();
                }
                item.Count = ReadShort();
                item.Aux = ReadVarInt();
                if (ReadBool()) //has stack network id
                {
                    if (network && protocolVersion < Info.v1_26_40)
                    {
                        ReadVarInt(); //net id variant type
                    }
                    ReadSignedVarInt(); //stack network id
                }
                item.BlockRuntimeId = network ? ReadVarInt() : ReadSignedVarInt();
                ReadBytes();//user data (nbt, can place on, can destroy). Useless for server auth inventory
                return item;
            }
            return new Items.VanillaItems.Air();
        }

        public List<Item> ReadCraftResultsDeprecatedItems()
        {
            var items = new List<Item>();

            var count = ReadVarInt();
            for (int i = 0; i < count; i++)
            {
                items.Add(protocolVersion >= Info.v1_26_40 ? ReadStackRequestItemDescriptor() : ReadItemInstance());
            }

            return items;
        }
        public Item ReadItemInstance()
        {
            var id = ReadSignedVarInt();
            if (id == 0 && protocolVersion < Info.v1_26_40)
            {
                return new Items.VanillaItems.Air(); //before 1.26.40 air was written as single varint
            }

            Item item = ItemPalette.items.GetValueOrDefault((short)id)?.Clone() ?? new Items.VanillaItems.Air();
            item.Count = ReadShort();
            item.Aux = ReadVarInt();
            item.BlockRuntimeId = ReadSignedVarInt();
            ReadBytes();//user data. Useless for server auth inventory
            return item;
        }
        public Item ReadStackRequestItemDescriptor()
        {
            int descriptorType = ReadVarInt();
            ReadByte(); //duplicated descriptor type
            Item item = new Items.VanillaItems.Air();
            int aux = 0;
            if (descriptorType != 0) //0 = invalid descriptor
            {
                string name = ReadString();
                aux = ReadSignedVarInt();
                item = ItemPalette.GetItem(name) ?? item;
            }
            item.Count = ReadShort();
            item.Aux = aux;
            item.BlockRuntimeId = ReadVarInt();
            ReadBytes(); //user data
            return item;
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
            if (protocolVersion >= Info.v1_26_20)
            {
                values.FrictionModifier = ReadFloat();
                values.Bounciness = ReadFloat();
                values.AirDragModifier = ReadFloat();
            }
            return values;
        }

        public ItemStackRequestSlotInfo ReadSlotInfo()
        {
            var slotInfo = new ItemStackRequestSlotInfo();
            slotInfo.ContainerName = ReadContainerName();
            slotInfo.Slot = ReadByte();
            slotInfo.NetIdVariant = protocolVersion >= Info.v1_26_40 ? ReadInt() : ReadSignedVarInt();
            return slotInfo;
        }

        public FullContainerName ReadContainerName()
        {
            var containerName = new FullContainerName();
            containerName.ContainerName = (ContainerEnumName)ReadByte();
            containerName.DynamicId = ReadBool() ? ReadInt() : null;
            return containerName;
        }

        public bool ReadPresence()
        {
            if (protocolVersion >= Info.v1_26_50)
            {
                return ReadBool();
            }
            return ReadBool() && ReadBool();
        }

        public List<PlayerBlockAction> ReadBlockActions()
        {
            var actions = new List<PlayerBlockAction>();
            bool v1_26_40 = protocolVersion >= Info.v1_26_40;
            var actionCount = v1_26_40 ? ReadVarInt() : ReadSignedVarInt();
            for (int i = 0; i < actionCount; i++)
            {
                var action = new PlayerBlockAction();
                action.PlayerActionType = ReadSignedVarInt();
                action.ActionType = (PlayerActionType)action.PlayerActionType;
                bool hasPosition = v1_26_40; //since 1.26.40 every action carries position and face
                switch (action.ActionType)
                {
                    case PlayerActionType.PredictDestroyBlock:
                    case PlayerActionType.StartDestroyBlock:
                    case PlayerActionType.AbortDestroyBlock:
                    case PlayerActionType.CrackBlock:
                    case PlayerActionType.ContinueDestroyBlock:
                        hasPosition = true;
                        break;
                }
                if (hasPosition)
                {
                    action.X = ReadSignedVarInt();
                    action.Y = ReadSignedVarInt();
                    action.Z = ReadSignedVarInt();
                    action.Facing = ReadSignedVarInt();
                }
                actions.Add(action);
            }
            return actions;
        }

        public List<LegacySlot> ReadLegacySlots(int legacyRequestId, bool newShape)
        {
            bool hasLegacySlots = true;
            var legacySlots = new List<LegacySlot>();

            if (newShape)
            {
                 hasLegacySlots = ReadBool();
            }

            if (hasLegacySlots && legacyRequestId < -1 && (legacyRequestId & 1) == 0)
            {
                int legacyCount = ReadVarInt();

                for (int i = 0; i < legacyCount; i++)
                {
                    LegacySlot legacySlot = new LegacySlot();
                    legacySlot.ContainerId = ReadByte();
                    legacySlot.Slot = ReadBytes();
                    legacySlots.Add(legacySlot);
                }
            }

            return legacySlots;
        }

        public List<TransactionAction> ReadTransactionActions(bool newShape)
        {
            var actions = new List<TransactionAction>();
            int count = ReadVarInt();

            for (int b = 0; b < count; b++)
            {
                TransactionAction action = new TransactionAction();

                action.Source = (SourceType)ReadVarInt();

                if (newShape)
                {
                    if (ReadPresence())
                    {
                        action.ContainerID = (sbyte)ReadByte();
                    }
                    if (ReadPresence())
                    {
                        action.Flags = (uint)ReadVarInt();
                    }
                    action.Slot = ReadVarInt();
                    action.ItemFrom = ReadItem(true);
                    action.ItemTo = ReadItem(true);
                }
                else
                {
                    switch (action.Source)
                    {
                        case SourceType.ContainerInventory:
                        case SourceType.UntrackedInteractionUI:
                        case SourceType.NonImplementedTodo:
                            action.ContainerID = ReadSignedVarInt();
                            break;
                        case SourceType.WorldInteraction:
                            action.Flags = (uint)ReadVarInt();
                            break;
                    }

                    action.Slot = ReadVarInt();
                    action.ItemFrom = ReadItem();
                    action.ItemTo = ReadItem();
                }
                actions.Add(action);
            }

            return actions;
        }

        public void ReadItemUseData(Transaction transaction, bool newShape)
        {
            transaction.ActionType = newShape ? ReadSignedVarInt() : ReadVarInt();
            transaction.TriggerType = newShape ? ReadByte() : ReadVarInt();
            transaction.BlockPosition = ReadBlockNetPos();
            transaction.Face = newShape ? ReadByte() : ReadSignedVarInt();
            transaction.Slot = ReadSignedVarInt();
            if (protocolVersion >= Info.v1_26_50)
            {
                transaction.Hand = ReadByte();
            }
            transaction.Item = ReadItem(newShape);
            transaction.PlayerPosition = ReadVec3();
            transaction.ClickPosition = ReadVec3();
            transaction.BlockRuntimeId = ReadVarInt();
            transaction.ClientPrediction = newShape ? ReadByte() : ReadVarInt();
            if (protocolVersion >= Info.v1_26_10)
            {
                transaction.ClientCooldownState = ReadByte();
            }
        }

        public List<ItemStack> ReadItemStack()
        {
            var stacks = new List<ItemStack>();
            var size = ReadVarInt();

            for (int i = 0; i < size; i++)
            {
                stacks.Add(ReadItemStackRequest());
            }
            return stacks;
        }

        public ItemStack ReadItemStackRequest()
        {
            var stack = new ItemStack();
            stack.RequestId = ReadVarInt();

            int actionCount = ReadVarInt();

            for (int a = 0; a < actionCount; a++)
            {
                ItemStackRequestActionType actionType;
                if (protocolVersion >= Info.v1_26_40)
                {
                    actionType = ItemStackRequestActionTypeMapper.FromNetworkId(ReadVarInt(), protocolVersion);
                    ReadByte(); //duplicated action type
                }
                else
                {
                    actionType = (ItemStackRequestActionType)ReadByte();
                }
                Log.debug($"Reading action type: {actionType}");
                stack.Actions.Add(ReadItemStackRequestAction(actionType));
            }
            int stringCount = ReadVarInt();
            for (int b = 0; b < stringCount; b++)
            {
                stack.StringToFilter.Add(ReadString());
            }

            stack.StringToFilterOrigin = ReadInt();
            return stack;
        }

        private Actions ReadItemStackRequestAction(ItemStackRequestActionType actionType)
        {
            bool v1_26_40 = protocolVersion >= Info.v1_26_40;
            Actions action;
            switch (actionType)
            {
                case ItemStackRequestActionType.Take:
                    action = new TakeAction() { Amount = ReadByte(), Source = ReadSlotInfo(), Destination = ReadSlotInfo() };
                    break;
                case ItemStackRequestActionType.Place:
                    action = new PlaceAction() { Amount = ReadByte(), Source = ReadSlotInfo(), Destination = ReadSlotInfo() };
                    break;
                case ItemStackRequestActionType.Swap:
                    action = new SwapAction() { Source = ReadSlotInfo(), Destination = ReadSlotInfo() };
                    break;
                case ItemStackRequestActionType.Drop:
                    action = new DropAction() { Amount = ReadByte(), Source = ReadSlotInfo(), Randomly = ReadBool() };
                    break;
                case ItemStackRequestActionType.Destroy:
                    action = new DestoryAction() { Amount = ReadByte(), Source = ReadSlotInfo() };
                    break;
                case ItemStackRequestActionType.Consume:
                    action = new ConsumeAction() { Amount = ReadByte(), Source = ReadSlotInfo() };
                    break;
                case ItemStackRequestActionType.Create:
                    action = new CreateAction() { ResultsSlot = ReadByte() };
                    break;
                case ItemStackRequestActionType.PlaceInItemContainer_DEPRECATED:
                case ItemStackRequestActionType.TakeFromItemContainer_DEPRECATED:
                    action = new TakeAction() { Amount = ReadByte(), Source = ReadSlotInfo(), Destination = ReadSlotInfo() };
                    break;
                case ItemStackRequestActionType.ScreenLabTableCombine:
                case ItemStackRequestActionType.CraftNonImplemented_DEPRECATEDASKTYLAING:
                    action = new EmptyAction();
                    break;
                case ItemStackRequestActionType.ScreenBeaconPayment:
                    action = new BeaconPaymentAction() { PrimaryEffect = ReadSignedVarInt(), SecondaryEffect = ReadSignedVarInt() };
                    break;
                case ItemStackRequestActionType.ScreenHUDMineBlock:
                    action = new MineBlockAction() { HotbarSlot = ReadSignedVarInt(), PredictedDurability = ReadSignedVarInt(), NetId = v1_26_40 ? ReadInt() : ReadSignedVarInt() };
                    break;
                case ItemStackRequestActionType.CraftRecipe:
                    action = new CraftRecipeAction() { RecipeNetId = ReadVarInt(), CraftCount = ReadByte() };
                    break;
                case ItemStackRequestActionType.CraftCreative:
                    action = new CraftCreativeAction() { ItemId = ReadVarInt(), CraftCount = ReadByte() };
                    break;
                case ItemStackRequestActionType.CraftRecipeOptional:
                    action = new CraftRecipeOptionalAction() { RecipeNetId = ReadVarInt(), FilteredStringIndex = ReadInt() };
                    break;
                case ItemStackRequestActionType.CraftRepairAndDisenchant:
                    action = new CraftRepairAndDisenchantAction() { RecipeNetId = v1_26_40 ? ReadInt() : ReadVarInt(), CraftCount = ReadByte(), Cost = ReadSignedVarInt() };
                    break;
                case ItemStackRequestActionType.CraftLoom:
                    action = new CraftLoomAction() { PatternId = ReadString(), CraftCount = ReadByte() };
                    break;
                case ItemStackRequestActionType.CraftReserved:
                    action = new CraftReservedAction() { Id = ReadString(), CraftCount = ReadByte() };
                    break;
                case ItemStackRequestActionType.CraftResults_DEPRECATEDASKTYLAING:
                    action = new CraftResults_DEPRECATEDASKTYLAING() { Items = ReadCraftResultsDeprecatedItems(), CraftCount = ReadByte() };
                    break;
                default:
                    throw new NotSupportedException($"Item stack request action {actionType} is not supported");
            }
            action.ActionsType = actionType;
            return action;
        }

        public T? ReadOptional<T>(Func<T> readFunction)
        {
            return ReadBool() ? readFunction() : default;
        }

        public List<TEnum> ReadLegacy<TEnum>() where TEnum : Enum
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

        public List<TEnum> Read<TEnum>() where TEnum : Enum
        {
            List<TEnum> result = new();

            bool hasFlags = ReadBool();
            if (!hasFlags)
            {
                return result;
            }

            int count = ReadVarInt();

            for (int i = 0; i < count; i++)
            {
                int flag = ReadSignedVarInt();

                result.Add((TEnum)Enum.ToObject(typeof(TEnum), flag));
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

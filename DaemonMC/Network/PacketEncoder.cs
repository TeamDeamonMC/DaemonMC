using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Text;
using DaemonMC.Biomes;
using DaemonMC.Entities;
using DaemonMC.Items;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;
using fNbt;

namespace DaemonMC.Network
{
    public class PacketEncoder
    {
        public readonly object Sync = new object();
        public IPEndPoint clientEp = null!;
        public int protocolVersion = 0;
        public MemoryStream byteStream;

        public PacketEncoder(IPEndPoint ep)
        {
            clientEp = ep;
            byteStream = new MemoryStream();
            protocolVersion = RakSessionManager.getSession(ep).protocolVersion;
        }

        public void handlePacket(string type = "bedrock")
        {
            byte[] trimmedBuffer = byteStream.ToArray()[..(int)byteStream.Position];

            if (type == "bedrock")
            {
                var session = RakSessionManager.getSession(clientEp);
                if (session == null) return;

                int packetID = ToDataTypes.ReadVarInt(trimmedBuffer);
                Log.packetOut(clientEp, (Info.Bedrock)packetID);

                bool useCompression = session.initCompression;
                byte[] bedrockId = useCompression ? new byte[] { (byte)DaemonMC.Compression } : new byte[] { };

                byte[] lengthVarInt = ToDataTypes.WriteVarint(trimmedBuffer.Length);

                byte[] toCompress = lengthVarInt.Concat(trimmedBuffer).ToArray();

                if (useCompression)
                {
                    switch (DaemonMC.Compression)
                    {
                        case CompressionTypes.ZLib:
                            toCompress = Compression.CompressZLib(toCompress);
                            break;
                        case CompressionTypes.Snappy:
                            toCompress = Compression.CompressSnappy(toCompress);
                            break;
                    }
                }

                byte[] finalPacket = bedrockId.Concat(toCompress).ToArray();

                ResetStream();

                if (session.encryptor != null && session.encryptor.validated)
                {
                    finalPacket = session.encryptor.Encrypt(finalPacket);
                }

                var packet = new GamePacket
                {
                    Payload = finalPacket
                };
                packet.EncodePacket(this);
                return;
            }


            Log.packetOut(clientEp, (Info.RakNet)trimmedBuffer[0]);

            ResetStream();

            if (trimmedBuffer[0] == 3)
            {
                Reliability.ReliabilityHandler(this, trimmedBuffer, ReliabilityType.unreliable);
            }
            else
            {
                Reliability.ReliabilityHandler(this, trimmedBuffer);
            }
        }

        private void ResetStream()
        {
            byteStream.SetLength(0);
            byteStream.Position = 0;
        }

        public void SendPacket(int pkid, bool pooled = true)
        {
            Server.DatGrOut++;
            if (pkid <= 127 || pkid >= 141) { Log.packetOut(clientEp, (Info.RakNet)pkid); };
            byte[] trimmedBuffer = new byte[byteStream.Position];
            Array.Copy(byteStream.ToArray(), trimmedBuffer, byteStream.Position);
            if (!RakSessionManager.sessions.TryGetValue(clientEp, out var session))
            {
                Log.warn($"Tried to send data to disconnected client {clientEp.Address}");
                return;
            }
            Server.Sock.SendTo(trimmedBuffer, clientEp);
            if (pkid == 128) { RakSessionManager.getSession(clientEp).sequenceNumber++; };
            if (pooled) { PacketEncoderPool.Return(this); }
        }

        public void Reset()
        {
            clientEp = null;
            byteStream.SetLength(0);
            byteStream.Position = 0;
        }

        public void PacketId(int id)
        {
            WriteVarInt((uint)id);
        }

        public void WriteBool(bool value)
        {
            byteStream.WriteByte(value ? (byte)1 : (byte)0);
        }

        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byteStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteIntBE(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            byteStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byteStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteVarInt_Signed(int rawValue)
        {
            uint value = unchecked((uint)rawValue);
            while ((value & 0xFFFFFF80) != 0)
            {
                byteStream.WriteByte((byte)((value & 127) | 128));
                value >>= 7;
            }
            byteStream.WriteByte((byte)value);
        }

        public void WriteVarInt(int value)
        {
            WriteVarInt((uint)value);
        }

        public void WriteVarInt(uint value)
        {
            if (value < 0)
            {
                throw new InvalidOperationException($"WriteVarInt was called with a negative value ({value})");
            }

            while ((value & -128) != 0)
            {
                byteStream.WriteByte((byte)((value & 127) | 128));
                value >>= 7;
            }
            byteStream.WriteByte((byte)(value & 127));
        }

        public void WriteSignedVarInt(int value)
        {
            uint zigzagEncoded = (uint)((value << 1) ^ (value >> 31));
            WriteVarInt(zigzagEncoded);
        }

        public void WriteShort(ushort value)
        {
            byteStream.WriteByte((byte)value);
            byteStream.WriteByte((byte)(value >> 8));
        }

        public void WriteShort(short value)
        {
            byteStream.WriteByte((byte)(value & 0xFF));
            byteStream.WriteByte((byte)((value >> 8) & 0xFF));
        }

        public void WriteShortBE(ushort value)
        {
            byteStream.WriteByte((byte)(value >> 8));
            byteStream.WriteByte((byte)value);
        }

        public void WriteByte(byte value)
        {
            byteStream.WriteByte(value);
        }

        public void WriteBytes(byte[] data, bool writeLength = true)
        {
            if (writeLength)
            {
                WriteVarInt(data.Count());
            }
            byteStream.Write(data, 0, data.Length);
        }

        public void WriteLongLE(long value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            Array.Reverse(valueBytes);
            byteStream.Write(valueBytes, 0, valueBytes.Length);
        }

        public void WriteLong(long value)
        {
            byte[] valueBytes = BitConverter.GetBytes(value);
            byteStream.Write(valueBytes, 0, valueBytes.Length);
        }

        public void WriteMagic(string magic)
        {
            for (int i = 0; i < magic.Length; i += 2)
            {
                string byteString = magic.Substring(i, 2);
                byte b = byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber);
                byteStream.WriteByte(b);
            }
        }

        public void WriteRakString(string str)
        {
            ushort length = (ushort)str.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);
            Array.Reverse(lengthBytes);
            byteStream.Write(lengthBytes, 0, lengthBytes.Length);

            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            byteStream.Write(strBytes, 0, strBytes.Length);
        }

        public void WriteString(string str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            WriteVarInt(strBytes.Length);
            byteStream.Write(strBytes, 0, strBytes.Length);
        }

        public void WriteAddress(string ip = "127.0.0.1")
        {
            string[] ipParts = ip.Split('.');
            byte[] ipAddress = new byte[] { byte.Parse(ipParts[0]), byte.Parse(ipParts[1]), byte.Parse(ipParts[2]), byte.Parse(ipParts[3]) };

            byteStream.WriteByte(4);

            byteStream.Write(ipAddress, 0, ipAddress.Length);

            byte[] portBytes = BitConverter.GetBytes((ushort)Server.Port);
            Array.Reverse(portBytes);
            byteStream.Write(portBytes, 0, portBytes.Length);
        }

        public void WriteUInt24LE(uint value)
        {
            byteStream.WriteByte((byte)(value & 0xFF));
            byteStream.WriteByte((byte)((value >> 8) & 0xFF));
            byteStream.WriteByte((byte)((value >> 16) & 0xFF));
        }

        public void WriteVarLong(long value)
        {
            WriteVarLong((ulong)value);
        }

        public void WriteVarLong(ulong value)
        {
            if (value < 0)
            {
                throw new InvalidOperationException($"WriteVarLong was called with a negative value ({value})");
            }

            while ((value & ~0x7FUL) != 0)
            {
                byteStream.WriteByte((byte)((value & 0x7FUL) | 0x80UL));
                value >>= 7;
            }
            byteStream.WriteByte((byte)(value & 0x7FUL));
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

            byteStream.Write(serializedTag, 0, serializedTag.Length);
        }

        public void WriteUUID(Guid uuid)
        {
            byte[] uuidBytes = uuid.ToByteArray();
            byte[] mostSignificantBits = uuidBytes.Take(8).Reverse().ToArray();
            byte[] leastSignificantBits = uuidBytes.Skip(8).Take(8).Reverse().ToArray();

            byte[] reordered = new byte[8];

            reordered[0] = mostSignificantBits[1];
            reordered[1] = mostSignificantBits[0];
            reordered[2] = mostSignificantBits[3];
            reordered[3] = mostSignificantBits[2];

            reordered[4] = mostSignificantBits[7];
            reordered[5] = mostSignificantBits[6];

            reordered[6] = mostSignificantBits[5];
            reordered[7] = mostSignificantBits[4];

            WriteBytes(reordered, false);
            WriteBytes(leastSignificantBits, false);
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

        public void WriteSkin(Skin skin)
        {
            WriteString(skin.SkinId);
            WriteString(skin.PlayFabId);
            WriteString(skin.SkinResourcePatch);
            WriteInt(skin.SkinImageWidth);
            WriteInt(skin.SkinImageHeight);
            WriteBytes(skin.SkinData);
            WriteInt(skin.AnimatedImageData.Count());
            foreach (var animation in skin.AnimatedImageData)
            {
                WriteInt(animation.ImageWidth);
                WriteInt(animation.ImageHeight);
                byte[] imageData = Convert.FromBase64String(animation.Image);
                WriteBytes(imageData);
                WriteInt(animation.Type);
                WriteFloat(animation.Frames);
                WriteInt(animation.AnimationExpression);
            }
            WriteInt(skin.Cape.CapeImageWidth);
            WriteInt(skin.Cape.CapeImageHeight);
            WriteBytes(skin.Cape.CapeData);
            WriteString(skin.SkinGeometryData);
            WriteString(skin.SkinGeometryDataEngineVersion);
            WriteString(skin.SkinAnimationData);
            WriteString(skin.Cape.CapeId);
            WriteString(skin.SkinId + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            WriteString(skin.ArmSize);
            WriteString(skin.SkinColor);
            WriteInt(skin.PersonaPieces.Count());
            foreach (var part in skin.PersonaPieces)
            {
                WriteString(part.PieceId);
                WriteString(part.PieceType);
                WriteString(part.PackId);
                WriteBool(part.IsDefault);
                WriteString(part.ProductId);
            }
            WriteInt(skin.PieceTintColors.Count());
            foreach (var part in skin.PieceTintColors)
            {
                WriteString(part.PieceType);
                WriteInt(part.Colors.Count());
                foreach (var color in part.Colors)
                {
                    WriteString(color);
                }
            }
            WriteBool(skin.PremiumSkin);
            WriteBool(skin.PersonaSkin);
            WriteBool(skin.CapeOnClassicSkin);
            WriteBool(false); //todo whats this?
            WriteBool(skin.OverrideSkin);
        }

        public void WriteMetadata(Dictionary<ActorData, Metadata> metadata)
        {
            WriteVarInt(metadata.Count);
            foreach (var entry in metadata)
            {
                WriteVarInt((uint)entry.Key);

                switch (entry.Value.Value)
                {
                    case byte value:
                        WriteVarInt(0);
                        WriteByte(value);
                        break;
                    case short value:
                        WriteVarInt(1);
                        WriteShort((ushort)value);
                        break;
                    case int value:
                        WriteVarInt(2);
                        WriteSignedVarInt(value);
                        break;
                    case float value:
                        WriteVarInt(3);
                        WriteFloat(value);
                        break;
                    case string value:
                        WriteVarInt(4);
                        WriteString(value);
                        break;
                    case NbtCompound value:
                        WriteVarInt(5);
                        WriteCompoundTag(value);
                        break;
                    /*case BlockPos value: //todo
                        WriteVarInt(6);
                        WriteBlockPos(value);
                        break;*/
                    case long value:
                        WriteVarInt(7);
                        WriteSignedVarLong(value);
                        break;
                    case Vector3 value:
                        WriteVarInt(8);
                        WriteVec3(value);
                        break;
                }
            }
        }

        public void WriteGameRulesData(Dictionary<string, GameRule> gamerule)
        {
            WriteVarInt(gamerule.Count);
            foreach (var entry in gamerule)
            {
                WriteString(entry.Key);
                WriteBool(true);

                switch (entry.Value.Value)
                {
                    case bool value:
                        WriteVarInt(1);
                        WriteBool(value);
                        break;
                    case int value:
                        WriteVarInt(2);
                        if (protocolVersion >= Info.v1_21_111)
                        {
                            WriteVarInt(value);
                        }
                        else
                        {
                            WriteSignedVarInt(value);
                        }
                        break;
                    case float value:
                        WriteVarInt(3);
                        WriteFloat(value);
                        break;
                }
            }
        }

        public void WriteEmotes(List<Guid> emoteIds)
        {
            WriteVarInt(emoteIds.Count);
            foreach (var emote in emoteIds)
            {
                WriteUUID(emote);
            }
        }

        public void WriteResourcePacksInfo(List<ResourcePack> packs)
        {
            WriteShort((ushort)packs.Count());
            foreach (var pack in packs)
            {
                if (protocolVersion >= Info.v1_21_50)
                {
                    WriteUUID(pack.UUID);
                }
                if (protocolVersion <= Info.v1_21_40)
                {
                    WriteString(pack.UUID.ToString());
                }
                WriteString(pack.PackIdVersion);
                WriteLong(pack.PackContent.Length);
                WriteString(pack.ContentKey);
                WriteString(pack.SubpackName);
                WriteString(pack.ContentId);
                WriteBool(pack.HasScripts);
                WriteBool(pack.IsAddon);
                WriteBool(pack.RayTracking);
                WriteString(pack.CdnUrl);
            }
        }

        public void WriteResourcePacksStack(List<ResourcePack> packs)
        {
            WriteVarInt(packs.Count);
            foreach (var pack in packs)
            {
                WriteString(pack.UUID.ToString());
                WriteString(pack.PackIdVersion);
                WriteString(pack.SubpackName);
            }
        }

        public void WritePlayerAttributes(List<AttributeValue> attributes)
        {
            WriteVarInt(attributes.Count);
            foreach (var attribute in attributes)
            {
                WriteFloat(attribute.MinValue);
                WriteFloat(attribute.MaxValue);
                WriteFloat(attribute.CurrentValue);
                WriteFloat(attribute.DefaultMinValue);
                WriteFloat(attribute.DefaultMaxValue);
                WriteFloat(attribute.DefaultValue);
                WriteString(attribute.Name);
                WriteVarInt(0); //todo modifiers
            }
        }

        public void WriteActorAttributes(List<AttributeValue> attributes)
        {
            WriteVarInt(attributes.Count);
            foreach (var attribute in attributes)
            {
                WriteString(attribute.Name);
                WriteFloat(attribute.MinValue);
                WriteFloat(attribute.CurrentValue);
                WriteFloat(attribute.MaxValue);
            }
        }

        public void WriteAbilitiesData(List<AbilitiesData> abilitiesDatas)
        {
            WriteVarInt(abilitiesDatas.Count);
            foreach (var data in abilitiesDatas)
            {
                WriteShort((ushort)data.Layer);
                WriteInt(data.AbilitiesSet);
                WriteAbilityValues(data.AbilityValues);
                WriteFloat(data.FlySpeed);
                if (protocolVersion >= Info.v1_21_60)
                {
                    WriteFloat(data.VerticalFlySpeed);
                }
                WriteFloat(data.WalkSpeed);
            }
        }

        public void WriteAbilityValues(PermissionSet permissions)
        {
            int value = 0;
            if (permissions.Build)
            {
                value |= (1 << (int)AbilitiesIndex.Build);
            }
            if (permissions.Mine)
            {
                value |= (1 << (int)AbilitiesIndex.Mine);
            }
            if (permissions.DoorsAndSwitches)
            {
                value |= (1 << (int)AbilitiesIndex.DoorsAndSwitches);
            }
            if (permissions.OpenContainers)
            {
                value |= (1 << (int)AbilitiesIndex.OpenContainers);
            }
            if (permissions.AttackPlayers)
            {
                value |= (1 << (int)AbilitiesIndex.AttackPlayers);
            }
            if (permissions.AttackMobs)
            {
                value |= (1 << (int)AbilitiesIndex.AttackMobs);
            }
            if (permissions.OperatorCommands)
            {
                value |= (1 << (int)AbilitiesIndex.OperatorCommands);
            }
            if (permissions.Teleport)
            {
                value |= (1 << (int)AbilitiesIndex.Teleport);
            }
            if (permissions.MayFly)
            {
                value |= (1 << (int)AbilitiesIndex.MayFly);
            }
            WriteInt(value);
        }

        public void WriteBlockNetPos(Vector3 position)
        {
            WriteSignedVarInt((int)position.X);
            WriteVarInt_Signed((int)position.Y); //what kind of data type even is this?
            WriteSignedVarInt((int)position.Z);
        }

        public void WriteProperties(SynchedProperties properties)
        {
            WriteVarInt(properties.intEntries.Count);
            foreach (var entry in properties.intEntries)
            {
                WriteVarInt(entry.Key);
                WriteSignedVarInt(entry.Value);
            }

            WriteVarInt(properties.floatEntries.Count);
            foreach (var entry in properties.floatEntries)
            {
                WriteVarInt(entry.Key);
                WriteFloat(entry.Value);
            }
        }

        public void WriteBiomesOld(List<Biome> biomes)
        {
            var BiomeData = new NbtCompound("");
            foreach (var biome in biomes)
            {
                BiomeData.Add(new NbtCompound(biome.BiomeName)
                {
                    new NbtFloat("ash", biome.BiomeData.AshDensity),
                    new NbtFloat("blue_spores", biome.BiomeData.BlueSporeDensity),
                    new NbtFloat("downfall", biome.BiomeData.Downfall),
                    new NbtFloat("red_spores", biome.BiomeData.RedSporeDensity),
                    new NbtFloat("temperature", biome.BiomeData.Temperature),
                    new NbtFloat("white_ash", biome.BiomeData.WhiteAshDensity),
                });
            }
            WriteCompoundTag(BiomeData);
        }

        public void WriteBiomes(List<Biome> biomes)
        {
            ushort BiomeIndex = 0;
            WriteVarInt(biomes.Count);
            foreach (var biome in biomes)
            {
                WriteShort(BiomeIndex);
                if (protocolVersion >= Info.v1_21_100)
                {
                    WriteShort(biome.BiomeData.BiomeID);
                }
                else
                {
                    WriteOptional();
                }
                WriteFloat(biome.BiomeData.Temperature);
                WriteFloat(biome.BiomeData.Downfall);
                if (protocolVersion >= Info.v1_21_111)
                {
                    WriteFloat(biome.BiomeData.FoliageSnow);
                }
                if (protocolVersion <= Info.v1_21_100)
                {
                    WriteFloat(biome.BiomeData.RedSporeDensity);
                    WriteFloat(biome.BiomeData.BlueSporeDensity);
                    WriteFloat(biome.BiomeData.AshDensity);
                    WriteFloat(biome.BiomeData.WhiteAshDensity);
                }
                WriteFloat(biome.BiomeData.Depth);
                WriteFloat(biome.BiomeData.Scale);
                WriteFloat(biome.BiomeData.WaterColor);
                WriteBool(biome.BiomeData.Rain);
                WriteOptional(); //biome tags. todo?
                WriteOptional(); //not needed, client side chunk gen not supported
                BiomeIndex++;
            }

            WriteVarInt(biomes.Count);
            foreach (var biome in biomes)
            {
                WriteString(biome.BiomeName);
            }
        }

        public void WriteActorLinks(List<EntityLink> links)
        {
            WriteVarInt(links.Count);
            foreach (var link in links)
            {
                WriteSignedVarLong(0);//old entity id? like when you switch from one to another? todo find out
                WriteSignedVarLong(link.EntityId);
                WriteByte(link.Type);
                WriteBool(true);
                WriteBool(true);
                WriteFloat(link.AngularVelocity);
            }
        }

        public void WriteContainerName(FullContainerName ContainerName)
        {
            WriteByte(ContainerName.ContainerName);
            WriteOptional(ContainerName.DynamicId == 0 ? null : () => WriteSignedVarInt(ContainerName.DynamicId));
        }

        public void WriteItem(Item item)
        {
            if (item is Items.VanillaItems.Air)
            {
                WriteSignedVarInt(0);
            }
            else
            {
                WriteSignedVarInt(item.Id);
                WriteShort(item.Count);
                WriteVarInt(item.Aux);
                WriteBool(false);
                WriteSignedVarInt(0); //block runtime id.
                WriteItemData(item.Data);
            }
        }

        public void WriteItemData(NbtCompound nbt)
        {
            if (nbt == null)
            {
                WriteByte(0);
                return;
            }

            nbt.Name = "";

            var file = new NbtFile(nbt)
            {
                BigEndian = false,
                UseVarInt = false
            };

            var itemData = new List<byte>();
            itemData.AddRange(new List<byte> { 0xFF, 0xFF, 0x01 });
            itemData.AddRange(file.SaveToBuffer(NbtCompression.None));
            itemData.AddRange(new List<byte> { 0x00, 0x00 });

            WriteBytes(itemData.ToArray(), true);
        }

        public void WriteOptional(Action writeFunction = null)
        {
            WriteBool(writeFunction != null);
            if (writeFunction != null)
            {
                writeFunction();
            }
        }
    }

    public class PacketEncoderPool
    {
        private static ConcurrentStack<PacketEncoder> pool = new ConcurrentStack<PacketEncoder>();
        public static int cached;
        public static int inUse;

     public static PacketEncoder Get(Player player)
     {
         return Get(player.ep);
     }

        public static PacketEncoder Get(IPEndPoint clientEp)
        {
            Interlocked.Increment(ref inUse);
            if (pool.TryPop(out var encoder))
            {
                encoder.clientEp = clientEp;
                encoder.protocolVersion = RakSessionManager.sessions.TryGetValue(clientEp, out var session) ? session.protocolVersion : Info.ProtocolVersion.Last();
                return encoder;
            }

            Interlocked.Increment(ref cached);
            return new PacketEncoder(clientEp);
        }

        public static void Return(PacketEncoder encoder)
        {
            encoder.Reset();
            pool.Push(encoder);
            Interlocked.Decrement(ref inUse);
        }
    }
}

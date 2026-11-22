using System.Collections.Concurrent;
using System.Net;
using System.Numerics;
using System.Text;
using DaemonMC.Biomes;
using DaemonMC.Blocks;
using DaemonMC.Entities;
using DaemonMC.Items;
using DaemonMC.Level.Format;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.NetherNet;
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

                if (clientEp is NetherNetEndPoint netherNetEp)
                {
                    // NetherNet send batch directly (no 0xFE), DTLS replace bedrock encryption
                    NetherNetTransport.Send(netherNetEp, finalPacket);
                    PacketEncoderPool.Return(this);
                    return;
                }

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

            if (clientEp is NetherNetEndPoint)
            {
                PacketEncoderPool.Return(this);
                return;
            }

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
            if (!RakSessionManager.sessions.TryGetValue(clientEp, out var session) || clientEp is NetherNetEndPoint)
            {
                Log.warn($"Tried to send data to disconnected client {clientEp.Address}");
                return;
            }
            if (session.isClient == true)
            {
                session.client.Sock.SendTo(trimmedBuffer, clientEp);
            }
            else
            {
                Server.Sock.SendTo(trimmedBuffer, clientEp);
            }
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

        public void WriteStringList(List<string> list)
        {
            WriteVarInt((ushort)list.Count());
            foreach (var value in list)
            {
                WriteString(value);
            }
        }

        public void WriteMTU(int mtu)
        {
            int payloadLength = mtu - 28;
            byte[] payload = new byte[payloadLength];
            byteStream.Write(payload, 0, payload.Length);
        }

        public void WriteAddress(IPAddressInfo info)
        {
            byte[] ipParts = info.IPAddress;
            byte[] ipAddress = new byte[] { ipParts[0], ipParts[1], ipParts[2], ipParts[3] };

            byteStream.WriteByte(4);

            byteStream.Write(ipAddress, 0, ipAddress.Length);

            byte[] portBytes = BitConverter.GetBytes(info.Port);
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

        public void WritePackNames(List<string> packs)
        {
            if (protocolVersion >= Info.v1_26_40)
            {
                WriteVarInt(packs.Count());
            }
            else
            {
                WriteShort((ushort)packs.Count());
            }
            foreach (var pack in packs)
            {
                WriteString(pack);
            }
        }

        public void WriteSkin(Skin skin)
        {
            bool v1_26_40 = protocolVersion >= Info.v1_26_40;

            WriteString(skin.SkinId);
            if (protocolVersion < Info.v1_26_60)
            {
                WriteString(skin.PlayFabId); //moved to PlayerList
            }
            WriteString(skin.SkinResourcePatch);
            WriteInt(skin.SkinImageWidth);
            WriteInt(skin.SkinImageHeight);
            WriteBytes(skin.SkinData);
            if (v1_26_40) { WriteVarInt(skin.AnimatedImageData.Count()); } else { WriteInt(skin.AnimatedImageData.Count()); }
            foreach (var animation in skin.AnimatedImageData)
            {
                WriteInt(animation.ImageWidth);
                WriteInt(animation.ImageHeight);
                byte[] imageData = Convert.FromBase64String(animation.Image);
                WriteBytes(imageData);
                if (v1_26_40) { WriteVarInt(animation.Type); } else { WriteInt(animation.Type); }
                WriteFloat(animation.Frames);
                if (v1_26_40) { WriteVarInt(animation.AnimationExpression); } else { WriteInt(animation.AnimationExpression); }
            }
            WriteInt(skin.Cape.CapeImageWidth);
            WriteInt(skin.Cape.CapeImageHeight);
            WriteBytes(skin.Cape.CapeData);
            WriteString(skin.SkinGeometryData);
            WriteString(skin.SkinGeometryDataEngineVersion);
            WriteString(skin.SkinAnimationData);
            WriteString(skin.Cape.CapeId);
            WriteString(skin.SkinId + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            if (v1_26_40)
            {
                WriteByte(skin.ArmSize); //0 slim, 1 wide
                WriteInt(skin.SkinColor);
                WriteVarInt(skin.PersonaPieces.Count());
            }
            else
            {
                WriteString(skin.ArmSize == 0 ? "slim" : "wide");
                WriteString(Skin.IntToColor(skin.SkinColor));
                WriteInt(skin.PersonaPieces.Count());
            }
            foreach (var part in skin.PersonaPieces)
            {
                WriteString(part.PieceId);
                if (v1_26_40)
                {
                    WriteInt((int)PersonaPieceTypesExtensions.Parse(part.PieceType));
                    WriteUUID(Guid.TryParse(part.PackId, out var packId) ? packId : Guid.Empty);
                }
                else
                {
                    WriteString(part.PieceType);
                    WriteString(part.PackId);
                }
                WriteBool(part.IsDefault);
                WriteString(part.ProductId);
            }
            if (v1_26_40) { WriteVarInt(skin.PieceTintColors.Count()); } else { WriteInt(skin.PieceTintColors.Count()); }
            foreach (var part in skin.PieceTintColors)
            {
                WriteString(part.PieceType);
                if (v1_26_40)
                {
                    for (int j = 0; j < 4; j++) //always 4 ARGB colors
                    {
                        WriteInt(j < part.Colors.Count ? Skin.ColorToInt(part.Colors[j]) : 0);
                    }
                }
                else
                {
                    WriteInt(part.Colors.Count());
                    foreach (var color in part.Colors)
                    {
                        WriteString(color);
                    }
                }
            }
            WriteBool(skin.PremiumSkin);
            WriteBool(skin.PersonaSkin);
            WriteBool(skin.CapeOnClassicSkin);
            WriteBool(false); //is primary user
            WriteBool(skin.OverrideSkin);
            if (v1_26_40)
            {
                WriteString(Skin.NormalizeTrustedFlag(skin.trustedSkinFlag));
                WriteString(skin.profileHash);
            }
        }

        public void WriteMetadata(Dictionary<ActorData, Metadata> metadata)
        {
            WriteVarInt(metadata.Count);
            foreach (var entry in metadata)
            {
                WriteVarInt((uint)entry.Key);
                WriteVarInt((uint)entry.Value.Type);
                if (protocolVersion >= Info.v1_26_40)
                {
                    WriteByte((byte)entry.Value.Type); //tagged variant index
                }

                switch (entry.Value.Value)
                {
                    case byte value:
                        WriteByte(value);
                        break;
                    case short value:
                        WriteShort((ushort)value);
                        break;
                    case int value:
                        WriteSignedVarInt(value);
                        break;
                    case float value:
                        WriteFloat(value);
                        break;
                    case string value:
                        WriteString(value);
                        break;
                    case NbtCompound value:
                        WriteCompoundTag(value);
                        break;
                    /*case BlockPos value: //todo
                        WriteBlockPos(value);
                        break;*/
                    case long value:
                        WriteSignedVarLong(value);
                        break;
                    case Vector3 value:
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
            if (protocolVersion >= Info.v1_26_40)
            {
                WriteVarInt(packs.Count());
            }
            else
            {
                WriteShort((ushort)packs.Count());
            }
            foreach (var pack in packs)
            {
                WriteUUID(pack.UUID);
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
                WriteFloat(data.VerticalFlySpeed);
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
            if (permissions.Invulnerable)
            {
                value |= (1 << (int)AbilitiesIndex.Invulnerable);
            }
            if (permissions.Flying)
            {
                value |= (1 << (int)AbilitiesIndex.Flying);
            }
            if (permissions.MayFly)
            {
                value |= (1 << (int)AbilitiesIndex.MayFly);
            }
            if (permissions.Instabuild)
            {
                value |= (1 << (int)AbilitiesIndex.Instabuild);
            }
            if (permissions.Muted)
            {
                value |= (1 << (int)AbilitiesIndex.Muted);
            }
            if (permissions.WorldBuilder)
            {
                value |= (1 << (int)AbilitiesIndex.WorldBuilder);
            }
            if (permissions.NoClip)
            {
                value |= (1 << (int)AbilitiesIndex.NoClip);
            }
            if (permissions.PrivilegedBuilder)
            {
                value |= (1 << (int)AbilitiesIndex.PrivilegedBuilder);
            }
            if (permissions.VerticalFlySpeed)
            {
                value |= (1 << (int)AbilitiesIndex.VerticalFlySpeed);
            }
            WriteInt(value);
        }

        public void WriteBlockNetPos(Vector3 position)
        {
            WriteSignedVarInt((int)position.X);
            if (protocolVersion >= Info.v1_26_10)
            {
                WriteSignedVarInt((int)position.Y);
            }
            else
            {
                WriteVarInt_Signed((int)position.Y);
            }
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

        public void WriteBiomes(List<Biome> biomes)
        {
            var strings = new List<string>(biomes.Count);
            var stringIndex = new Dictionary<string, ushort>();
            ushort IndexOf(string value)
            {
                if (!stringIndex.TryGetValue(value, out ushort index))
                {
                    index = (ushort)strings.Count;
                    strings.Add(value);
                    stringIndex[value] = index;
                }
                return index;
            }
            foreach (var biome in biomes)
            {
                IndexOf(biome.BiomeName);
            }

            WriteVarInt(biomes.Count);
            foreach (var biome in biomes)
            {
                WriteShort(IndexOf(biome.BiomeName));
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
                WriteInt(biome.BiomeData.WaterColor);
                WriteBool(biome.BiomeData.Rain);
                var tags = biome.BiomeData.Tags;
                WriteOptional(tags.Count == 0 ? null : () =>
                {
                    WriteVarInt(tags.Count);
                    foreach (var tag in tags)
                    {
                        WriteShort(IndexOf(tag));
                    }
                });
                WriteOptional(); //not needed, client side chunk gen not supported
            }

            WriteVarInt(strings.Count);
            foreach (var value in strings)
            {
                WriteString(value);
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
            WriteByte((byte)ContainerName.ContainerName);
            WriteOptional(ContainerName.DynamicId == null || ContainerName.DynamicId == 0 ? null : () => WriteInt((int)ContainerName.DynamicId));
        }

        public int GetItemBlockRuntimeId(Item item)
        {
            if (item.BlockRuntimeId == 0 || !StateConverter.NeedsUpgrade(protocolVersion))
            {
                return item.BlockRuntimeId;
            }

            return BlockPalette.blockHashes.TryGetValue(item.BlockRuntimeId, out var block) ? block.GetHash(protocolVersion) : item.BlockRuntimeId;
        }

        public void WriteItemInstance(Item item, bool network = false)
        {
            if (!network && item is Items.VanillaItems.Air)
            {
                WriteSignedVarInt(0);
                if (protocolVersion >= Info.v1_26_40) //air is no longer a single varint
                {
                    WriteShort((ushort)0);
                    WriteVarInt(0);
                    WriteSignedVarInt(0);
                    WriteVarInt(0);
                }
            }
            else
            {
                if (network)
                {
                    WriteShort(item.Id);
                }
                else
                {
                    WriteSignedVarInt(item.Id);
                }
                WriteShort(item.Count);
                WriteVarInt(item.Aux);
                WriteSignedVarInt(GetItemBlockRuntimeId(item));
                WriteItemData(item.Data);
            }
        }

        public void WriteNetItemStack(Item item)
        {
            WriteItemStack(item, true);
        }

        public void WriteItemStack(Item item, bool network = false)
        {
            bool air = item is Items.VanillaItems.Air;
            if (!network && air)
            {
                WriteSignedVarInt(0);
            }
            else if (network && air && protocolVersion >= Info.v1_26_40)
            {
                WriteShort((ushort)0);
                WriteShort((ushort)0);
                WriteVarInt(0);
                WriteBool(false); //has stack net id
                WriteVarInt(0); //block runtime id
                WriteVarInt(0); //user data
            }
            else
            {
                if (network)
                {
                    WriteShort(item.Id);
                }
                else
                {
                    WriteSignedVarInt(item.Id);
                }
                WriteShort(item.Count);
                WriteVarInt(item.Aux);
                WriteBool(false); //has stack net id
                if (network)
                {
                    WriteVarInt(unchecked((uint)GetItemBlockRuntimeId(item)));
                }
                else
                {
                    WriteSignedVarInt(GetItemBlockRuntimeId(item));
                }
                WriteItemData(item.Data);
            }
        }

        public void WriteItemData(NbtCompound? nbt)
        {
            using var userData = new MemoryStream();
            using var writer = new BinaryWriter(userData);

            if (nbt == null)
            {
                writer.Write((short)0);
            }
            else
            {
                nbt.Name = "";

                var file = new NbtFile(nbt)
                {
                    BigEndian = false,
                    UseVarInt = false
                };

                byte[] nbtBytes = file.SaveToBuffer(NbtCompression.None);

                writer.Write((short)-1);
                writer.Write((byte)1);
                writer.Write(nbtBytes);
            }

            // CanPlaceOn count
            writer.Write(0);

            // CanDestroy count
            writer.Write(0);

            WriteBytes(userData.ToArray(), true);
        }

        public void WriteVoxelShapes(List<VoxelShape> shapes, VanillaVoxelShapes? vanilla = null)
        {
            WriteVarInt((vanilla?.ShapeCount ?? 0) + shapes.Count);
            if (vanilla != null)
            {
                WriteBytes(vanilla.Shapes, false);
            }
            foreach (var shape in shapes)
            {
                foreach (var cells in shape.Cells)
                {
                    WriteByte(cells.Xsize);
                    WriteByte(cells.Ysize);
                    WriteByte(cells.Zsize);

                    WriteVarInt(cells.Storage.Count);
                    foreach (var storage in cells.Storage)
                    {
                        WriteByte(storage);
                    }
                }

                WriteVarInt(shape.Coordinates.Count);
                foreach (var coordinate in shape.Coordinates)
                {
                    WriteFloat(coordinate.X);
                }

                WriteVarInt(shape.Coordinates.Count);
                foreach (var coordinate in shape.Coordinates)
                {
                    WriteFloat(coordinate.Y);
                }

                WriteVarInt(shape.Coordinates.Count);
                foreach (var coordinate in shape.Coordinates)
                {
                    WriteFloat(coordinate.Z);
                }
            }
        }

        public void WriteVoxelNameMap(List<VoxelShape> shapes, VanillaVoxelShapes? vanilla = null)
        {
            WriteVarInt((vanilla?.NameMap.Count ?? 0) + shapes.Count);
            if (vanilla != null)
            {
                foreach (var (name, handle) in vanilla.NameMap)
                {
                    WriteString(name);
                    WriteShort(handle);
                }
            }
            short id = (short)(vanilla?.ShapeCount ?? 0);
            foreach (var shape in shapes)
            {
                WriteString(shape.Name);
                WriteShort(id);
                id++;
            }
        }

        public void WriteItemStackResponse(List<ItemStackResponseInfo> itemStackResponse)
        {
            WriteVarInt(itemStackResponse.Count);
            foreach (var response in itemStackResponse)
            {
                WriteByte(response.Result);
                WriteVarInt(response.RequestId);
                if (protocolVersion >= Info.v1_26_40)
                {
                    if (protocolVersion < Info.v1_26_50)
                    {
                        WriteBool(true);
                    }
                    WriteBool(false); //containers. todo
                }
                else if (response.Result == 0) //success
                {
                    WriteVarInt(0); //containers. todo
                }
            }
        }

        public void WriteHeightMap(PacketEncoder encoder, HeightMapDataType type, sbyte[] data)
        {
            encoder.WriteByte((byte)type);
            bool hasData = type == HeightMapDataType.HasData;
            encoder.WriteBool(hasData);
            if (!hasData) return;

            byte[] bytes = (byte[])(Array)data;
            if (encoder.protocolVersion >= Info.v1_26_50)
            {
                for (int row = 0; row < 256; row += 16)
                {
                    encoder.WriteBytes(bytes[row..(row + 16)]);
                }
            }
            else
            {
                encoder.WriteBytes(bytes, false);
            }
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

     public static PacketEncoder Get(Player player)
     {
         return Get(player.ep);
     }

        public static PacketEncoder Get(IPEndPoint clientEp)
        {
            if (pool.TryPop(out var encoder))
            {
                encoder.clientEp = clientEp;
                encoder.protocolVersion = RakSessionManager.sessions.TryGetValue(clientEp, out var session) ? session.protocolVersion : Info.ProtocolVersion.Last();
                return encoder;
            }

            return new PacketEncoder(clientEp);
        }

        public static void Return(PacketEncoder encoder)
        {
            encoder.Reset();
            pool.Push(encoder);
        }
    }
}

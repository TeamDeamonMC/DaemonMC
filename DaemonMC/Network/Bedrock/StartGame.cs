using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class StartGamePacket
    {
        public long EntityId { get; set; }
        public int GameMode { get; set; }
        public Vector3 position { get; set; }
        public Vector2 rotation { get; set; }
        public long seed { get; set; }
        public ushort biomeType { get; set; } = 0;
        public string biomeName { get; set; } = "plains";
        public int dimension { get; set; } = 0;
        public int generator { get; set; }
        public int gameType { get; set; }
        public int difficulty { get; set; }
        public int spawnBlockX { get; set; }
        public int spawnBlockY { get; set; }
        public int spawnBlockZ { get; set; }
        public int editorType { get; set; }
        public int stopTime { get; set; }
    }

    public class StartGame
    {
        public static int id = 11;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(StartGamePacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteSignedVarLong(fields.EntityId);
            encoder.WriteVarLong((ulong)fields.EntityId);
            encoder.WriteVarInt(fields.GameMode);
            encoder.WriteVec3(fields.position);
            encoder.WriteVec2(fields.rotation);
            //Level settings
            encoder.WriteLongLE(fields.seed);
                    //Spawn settings
                    encoder.WriteShort(fields.biomeType);
                    encoder.WriteString(fields.biomeName);
                    encoder.WriteVarInt(fields.dimension);
                    //End of Spawn settings
                encoder.WriteSignedVarInt(fields.generator);
                encoder.WriteSignedVarInt(fields.gameType);
                encoder.WriteBool(false); //hardcore
                encoder.WriteSignedVarInt(fields.difficulty);
                encoder.WriteSignedVarInt(fields.spawnBlockX);
                encoder.WriteVarInt(fields.spawnBlockY);
                encoder.WriteSignedVarInt(fields.spawnBlockZ);
                encoder.WriteBool(false); //achievements
                encoder.WriteBool(false);
                encoder.WriteBool(false); //editorCreated
                encoder.WriteBool(false); //editorExported
                encoder.WriteSignedVarInt(fields.stopTime);
                encoder.WriteSignedVarInt(0);
                encoder.WriteBool(false);
                encoder.WriteString("");
                encoder.WriteFloat(0);
                encoder.WriteFloat(0);
                encoder.WriteBool(true); //platform content
                encoder.WriteBool(true); //multiplayer?
                encoder.WriteBool(true); //lan?
                encoder.WriteVarInt(0); //xbox broadcast settings
                encoder.WriteVarInt(0); //platform broadcast settings
                encoder.WriteBool(true); //commands?
                encoder.WriteBool(false); //texture packs?
                encoder.WriteVarInt(0); //game rules
                encoder.WriteInt(0); //experiments
                encoder.WriteBool(false);
                encoder.WriteBool(false); //bonus chest
                encoder.WriteBool(false); //map
                encoder.WriteByte(0); //permission level
                encoder.WriteInt(0); //chunk tick range
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(true);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteString(Info.version);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteBool(false);
                    encoder.WriteString("");
                    encoder.WriteString("");
                encoder.WriteBool(false);
                encoder.WriteBool(true);
                encoder.WriteBool(false);
            //End of Level settings
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteBool(false); //trial //ok
                //synced movement settings
                encoder.WriteSignedVarInt(0);
                encoder.WriteSignedVarInt(0);
                encoder.WriteBool(true);
                //end of synced movement settings
            encoder.WriteLong(0);
            encoder.WriteSignedVarInt(0);
            encoder.WriteVarInt(0); //block
            encoder.WriteVarInt(0); //item
            encoder.WriteString("");
            encoder.WriteBool(true); //new inventory
            encoder.WriteString(Info.version);
            encoder.WriteCompoundTag(new fNbt.NbtCompound(""));
            encoder.WriteLong(0); //blockstate checksum
            var uuid = Guid.NewGuid();
            encoder.WriteUUID(uuid);
            encoder.WriteBool(true);
            encoder.WriteBool(false);
            encoder.WriteBool(true);
            encoder.handlePacket();
        }
    }
}

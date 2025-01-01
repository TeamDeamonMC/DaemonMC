using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class StartGame
    {
        public Info.Bedrock id = Info.Bedrock.StartGame;

        public string LevelName;
        public long EntityId = 0;
        public int GameMode = 0;
        public Vector3 Position = new Vector3();
        public Vector2 Rotation = new Vector2();
        public long Seed = 0;
        public ushort BiomeType = 0;
        public string BiomeName = "plains";
        public int Dimension = 0;
        public int Generator = 1;
        public int GameType = 0;
        public int Difficulty = 0;
        public int SpawnBlockX = 0;
        public int SpawnBlockY = 0;
        public int SpawnBlockZ = 0;
        public int EditorType = 0;
        public int StopTime = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong((ulong) EntityId);
            encoder.WriteVarLong((ulong) EntityId);
            encoder.WriteVarInt(GameMode);
            encoder.WriteVec3(Position);
            encoder.WriteVec2(Rotation);
            //Level settings
                encoder.WriteLongLE(Seed);
                    //Spawn settings
                    encoder.WriteShort(BiomeType);
                    encoder.WriteString(BiomeName);
                    encoder.WriteVarInt(Dimension);
                    //End of Spawn settings
                encoder.WriteSignedVarInt(Generator);
                encoder.WriteSignedVarInt(GameType);
                encoder.WriteBool(false); //hardcore
                encoder.WriteSignedVarInt(Difficulty);
                encoder.WriteSignedVarInt(SpawnBlockX);
                encoder.WriteVarInt(SpawnBlockY);
                encoder.WriteSignedVarInt(SpawnBlockZ);
                encoder.WriteBool(false); //achievements
                encoder.WriteBool(false);
                encoder.WriteBool(false); //editorCreated
                encoder.WriteBool(false); //editorExported
                encoder.WriteSignedVarInt(StopTime);
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
                encoder.WriteByte(2); //permission level
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
                encoder.WriteBool(false);
                encoder.WriteBool(false);
            //End of Level settings
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteString(LevelName); //level name?
            encoder.WriteString("");
            encoder.WriteBool(false); //trial //ok
                //synced movement settings
                encoder.WriteSignedVarInt(0); //0 server auth off, need fix
                encoder.WriteSignedVarInt(40);
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
            encoder.WriteBool(false);
            encoder.WriteBool(true); //we use hashed block ids
            encoder.WriteBool(true);
            encoder.handlePacket();
        }
    }
}

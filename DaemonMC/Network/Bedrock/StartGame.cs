using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class StartGame : Packet
    {
        public override int Id => (int) Info.Bedrock.StartGame;

        public string LevelName = "";
        public long EntityId { get; set; } = 0;
        public int GameMode { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public long Seed { get; set; } = 0;
        public ushort BiomeType { get; set; } = 0;
        public string BiomeName { get; set; } = "plains";
        public int Dimension { get; set; } = 0;
        public int Generator { get; set; } = 1;
        public int GameType { get; set; } = 0;
        public int Difficulty { get; set; } = 0;
        public int SpawnBlockX { get; set; } = 0;
        public int SpawnBlockY { get; set; } = 0;
        public int SpawnBlockZ { get; set; } = 0;
        public int EditorType { get; set; } = 0;
        public int StopTime { get; set; } = 0;
        public bool TickDeathSystems { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarLong(EntityId);
            encoder.WriteVarLong(EntityId);
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
                encoder.WriteSignedVarInt(SpawnBlockY);
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
                encoder.WriteByte(3); //permission level
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
                encoder.WriteString(Info.Version);
                encoder.WriteInt(0);
                encoder.WriteInt(0);
                encoder.WriteBool(false);
                    encoder.WriteString("");
                    encoder.WriteString("");
                encoder.WriteBool(false);
                encoder.WriteBool(false);
                encoder.WriteBool(false);
            //End of Level settings
            if (encoder.protocolVersion < Info.v1_26_0)
            {
                encoder.WriteString("");
                encoder.WriteString("");
                encoder.WriteString("");
                encoder.WriteString("");
            }
            encoder.WriteString(LevelName); //level name?
            encoder.WriteString("");
            encoder.WriteString(""); //owner id idk
            encoder.WriteBool(false); //trial //ok
                //synced movement settings
                encoder.WriteSignedVarInt(40);
                encoder.WriteBool(true);
                //end of synced movement settings
            encoder.WriteLong(0);
            encoder.WriteSignedVarInt(0);
            encoder.WriteVarInt(0); //block
            encoder.WriteString("");
            encoder.WriteBool(true); //new inventory
            encoder.WriteString(Info.Version);
            encoder.WriteCompoundTag(new fNbt.NbtCompound(""));
            encoder.WriteLong(0); //blockstate checksum
            var uuid = Guid.NewGuid();
            encoder.WriteUUID(uuid);
            encoder.WriteBool(false);
            encoder.WriteBool(true); //we use hashed block ids
            if (encoder.protocolVersion >= Info.v1_21_100 && encoder.protocolVersion < Info.v1_21_130)
            {
                encoder.WriteBool(TickDeathSystems);
            }
            encoder.WriteBool(true);
            if (encoder.protocolVersion >= Info.v1_26_0)
            {
                encoder.WriteBool(false);
                encoder.WriteString("");
                encoder.WriteString("");
                encoder.WriteString("");
                encoder.WriteString("");
            }
        }
    }
}

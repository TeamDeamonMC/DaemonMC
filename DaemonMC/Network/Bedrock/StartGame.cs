using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class StartGame
    {
        public Info.Bedrock id = Info.Bedrock.StartGame;

        public ulong EntityId = 0;
        public int GameMode = 0;
        public Vector3 position = new Vector3();
        public Vector2 rotation = new Vector2();
        public long seed = 0;
        public ushort biomeType = 0;
        public string biomeName = "plains";
        public int dimension = 0;
        public int generator = 1;
        public int gameType = 0;
        public int difficulty = 0;
        public int spawnBlockX = 0;
        public int spawnBlockY = 0;
        public int spawnBlockZ = 0;
        public int editorType = 0;
        public int stopTime = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong(EntityId);
            encoder.WriteVarLong(EntityId);
            encoder.WriteVarInt(GameMode);
            encoder.WriteVec3(position);
            encoder.WriteVec2(rotation);
            //Level settings
            encoder.WriteLongLE(seed);
                    //Spawn settings
                    encoder.WriteShort(biomeType);
                    encoder.WriteString(biomeName);
                    encoder.WriteVarInt(dimension);
                    //End of Spawn settings
                encoder.WriteSignedVarInt(generator);
                encoder.WriteSignedVarInt(gameType);
                encoder.WriteBool(false); //hardcore
                encoder.WriteSignedVarInt(difficulty);
                encoder.WriteSignedVarInt(spawnBlockX);
                encoder.WriteVarInt(spawnBlockY);
                encoder.WriteSignedVarInt(spawnBlockZ);
                encoder.WriteBool(false); //achievements
                encoder.WriteBool(false);
                encoder.WriteBool(false); //editorCreated
                encoder.WriteBool(false); //editorExported
                encoder.WriteSignedVarInt(stopTime);
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
            encoder.WriteString("");
            encoder.WriteString("");
            encoder.WriteBool(false); //trial //ok
                //synced movement settings
                encoder.WriteSignedVarInt(3);
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
            encoder.WriteBool(false);
            encoder.WriteBool(true);
            encoder.handlePacket();
        }
    }
}

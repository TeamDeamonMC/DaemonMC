namespace DaemonMC.Network.Bedrock
{
    public class LevelChunk
    {
        public Info.Bedrock id = Info.Bedrock.LevelChunk;

        public int chunkX = 0;
        public int chunkZ = 0;
        public int count = 0;
        public byte[] data = new byte[0];

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteSignedVarInt(chunkX);
            encoder.WriteSignedVarInt(chunkZ);
            encoder.WriteSignedVarInt(0);
            encoder.WriteVarInt(count);
            encoder.WriteBool(false);

            encoder.WriteVarInt(data.Count());
            foreach (var raw in data)
            {
                encoder.WriteByte(raw);
            }

            encoder.handlePacket();
        }
    }
}

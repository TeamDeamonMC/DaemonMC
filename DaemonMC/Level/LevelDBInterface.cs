using DaemonMC.Utils;
using MiNET.LevelDB;

namespace DaemonMC.Level
{
    public class LevelDBInterface
    {
        public Chunk GetChunk(string levelName, int x, int z)
        {
            using var db = new Database(new DirectoryInfo($"Worlds/{levelName}.mcworld"));
            db.Open();

            byte[] index = ToDataTypes.GetByteSum(BitConverter.GetBytes(x), BitConverter.GetBytes(z));
            byte[] version = db.Get(ToDataTypes.GetByteSum(index, new byte[] { 0x76 }));
            byte[] dataKey = ToDataTypes.GetByteSum(index, new byte[] { 0x2f, 0 });

            var chunk = new Chunk();

            for (int y = 0; y < 24; y++) //since 1.18 320 up, -64 down. 64/16=4 negative subchunks. 320/16=20 positive subchunks. max 24 chunks.
            {
                dataKey[^1] = (byte)(y - 4);
                byte[] subChunk = db.Get(dataKey);

                if (subChunk != null)
                {
                    chunk.chunks.Add(ChunkUtils.DecodeSubChunk(subChunk));
                }
                else
                {
                    return chunk;
                }
            }
            db.Dispose();
            db.Close();
            return chunk;
        }
    }
}

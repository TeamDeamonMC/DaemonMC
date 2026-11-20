using DaemonMC.Blocks;
using DaemonMC.Utils;

namespace DaemonMC.Level.Generators
{
    public class SuperFlat
    {
        public Block[] Layers { get; set; } = [new Bedrock(), new Dirt(), new Dirt(), new GrassBlock()];

        public Chunk GenerateChunk()
        {
            var chunk = new Chunk();
            int subChunkCount = Math.Max(1, (int)Math.Ceiling(Layers.Length / 16f));

            for (int s = 0; s < subChunkCount; s++)
            {
                var subChunk = new SubChunk
                {
                    StorageSize = 1,
                };
                subChunk.Palette.Add(new Air().GetState());

                for (int localY = 0; localY < 16; localY++)
                {
                    int layer = s * 16 + localY;
                    if (layer >= Layers.Length) { break; }

                    var state = Layers[layer].GetState();
                    int paletteIndex = NbtUtils.IndexOf(subChunk.Palette, state);
                    if (paletteIndex == -1)
                    {
                        subChunk.Palette.Add(state);
                        paletteIndex = subChunk.Palette.Count - 1;
                    }

                    for (int x = 0; x < 16; x++)
                    {
                        for (int z = 0; z < 16; z++)
                        {
                            subChunk.Blocks[localY | (z << 4) | (x << 8)] = (byte)paletteIndex;
                        }
                    }
                }

                subChunk.BitsPerBlock = ChunkUtils.GetBitsPerBlock(subChunk.Palette.Count);
                chunk.Chunks.Add(subChunk);
            }

            return chunk;
        }
    }
}

using DaemonMC.Blocks;
using DaemonMC.Utils;

namespace DaemonMC.Level.Generators
{
    public class SuperFlat
    {
        public Block[] Layers { get; set; } = [new Bedrock(), new Dirt(), new Dirt(), new GrassBlock()];
        public int SubChunkCount => Math.Max(1, (int)Math.Ceiling(Layers.Length / 16f));

        public SubChunk GenerateSubChunk(int index)
        {
            var subChunk = new SubChunk();
            if (index < 0 || index >= SubChunkCount)
            {
                return subChunk;
            }

            subChunk.StorageSize = 1;
            subChunk.Palette.Add(new Air().GetState());
            var blocks = subChunk.Blocks;

            for (int localY = 0; localY < 16; localY++)
            {
                int layer = index * 16 + localY;
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
                        blocks[SubChunk.Index(x, localY, z)] = (byte)paletteIndex;
                    }
                }
            }

            subChunk.BitsPerBlock = ChunkUtils.GetBitsPerBlock(subChunk.Palette.Count);
            return subChunk;
        }
    }
}

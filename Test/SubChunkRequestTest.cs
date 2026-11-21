using DaemonMC.Blocks;
using DaemonMC.Level;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using MiNET.LevelDB.Utils;

namespace Test
{
    [TestClass]
    public class SubChunkRequestTest
    {
        private static Chunk BuildChunk()
        {
            var chunk = new Chunk();
            for (int i = 0; i < 3; i++)
            {
                var sub = ChunkUtils.CreateEmptySubChunk();
                sub.BiomePalette.Add(1);
                chunk.Chunks.Add(sub);
            }

            chunk.Chunks[0].Palette.Add(new Stone().GetState());
            Array.Fill(chunk.Chunks[0].Blocks, (byte)1);

            chunk.Chunks[1].Palette.Add(new Stone().GetState());
            for (int y = 0; y <= 4; y++)
            {
                chunk.Chunks[1].Blocks[(3 << 8) | (5 << 4) | y] = 1;
            }
            return chunk;
        }

        [TestMethod]
        public void AirDetectionAndHighest()
        {
            var chunk = BuildChunk();
            Assert.IsFalse(chunk.IsSubChunkAir(0));
            Assert.IsFalse(chunk.IsSubChunkAir(1));
            Assert.IsTrue(chunk.IsSubChunkAir(2));
            Assert.IsTrue(chunk.IsSubChunkAir(10)); //missing above stored ones
            Assert.AreEqual(1, chunk.GetHighestNonAirSubChunk());
            Assert.AreEqual(-1, new Chunk().GetHighestNonAirSubChunk());
        }

        [TestMethod]
        public void HeightMaps()
        {
            var chunk = BuildChunk();
            var heights = chunk.GetColumnHeights();
            Assert.AreEqual(16 + 4, heights[(5 << 4) | 3]);
            Assert.AreEqual(15, heights[0]);

            Assert.AreEqual(HeightMapDataType.HasData, Chunk.BuildSubChunkHeightMap(heights, 0, out var hm0));
            Assert.AreEqual(16, hm0[(5 << 4) | 3]); //column continues above
            Assert.AreEqual(15, hm0[0]);

            Assert.AreEqual(HeightMapDataType.HasData, Chunk.BuildSubChunkHeightMap(heights, 1, out var hm1));
            Assert.AreEqual(4, hm1[(5 << 4) | 3]);
            Assert.AreEqual(-1, hm1[0]);

            Assert.AreEqual(HeightMapDataType.AllTooLow, Chunk.BuildSubChunkHeightMap(heights, 2, out _));

            var allHigh = Enumerable.Repeat(100, 256).ToArray();
            Assert.AreEqual(HeightMapDataType.AllTooHigh, Chunk.BuildSubChunkHeightMap(allHigh, 0, out _));
        }

        [TestMethod]
        public void BiomePayload()
        {
            var chunk = BuildChunk();
            var data = chunk.NetworkSerializeBiomes(Dimension.Overworld);
            var stream = new MemoryStream(data);

            //single value storage, 0 bits
            Assert.AreEqual(1, stream.ReadByte());
            Assert.AreEqual(1, VarInt.ReadSInt32(stream));

            //remaining 23 identical -> "same as previous"
            for (int i = 1; i < Dimension.Overworld.SubChunkCount(); i++)
            {
                Assert.AreEqual(0xFF, stream.ReadByte(), $"biome {i}");
            }
            Assert.AreEqual(0, stream.ReadByte()); //border blocks
            Assert.AreEqual(stream.Length, stream.Position);
        }

        [TestMethod]
        public void SubChunkPayload()
        {
            var chunk = BuildChunk();
            var data = chunk.NetworkSerializeSubChunk(1, -3);
            var stream = new MemoryStream(data);

            Assert.AreEqual(9, stream.ReadByte()); //version
            Assert.AreEqual(1, stream.ReadByte()); //layers
            Assert.AreEqual(unchecked((byte)(sbyte)-3), stream.ReadByte()); //y index

            int flag = stream.ReadByte();
            Assert.AreEqual(1, flag & 1);
            int bits = flag >> 1;
            Assert.AreEqual(1, bits);

            int blocksPerWord = 32 / bits;
            int words = (4096 + blocksPerWord - 1) / blocksPerWord;
            var indices = new int[4096];
            int pos = 0;
            for (int w = 0; w < words; w++)
            {
                uint word = DaemonMC.ToDataTypes.ReadUInt32(stream);
                for (int b = 0; b < blocksPerWord && pos < 4096; b++, pos++)
                {
                    indices[pos] = (int)((word >> (b * bits)) & ((1u << bits) - 1));
                }
            }
            Assert.AreEqual(2, VarInt.ReadSInt32(stream)); //palette size
            VarInt.ReadSInt32(stream);
            VarInt.ReadSInt32(stream);
            Assert.AreEqual(stream.Length, stream.Position);

            Assert.AreEqual(1, indices[(3 << 8) | (5 << 4) | 4]);
            Assert.AreEqual(0, indices[(3 << 8) | (5 << 4) | 5]);
            Assert.AreEqual(5, indices.Count(i => i == 1));
        }

        private static fNbt.NbtCompound MakeBlockEntity(string id, int x, int y, int z)
        {
            return new fNbt.NbtCompound("")
            {
                new fNbt.NbtString("id", id),
                new fNbt.NbtInt("x", x),
                new fNbt.NbtInt("y", y),
                new fNbt.NbtInt("z", z),
                new fNbt.NbtList("Items", fNbt.NbtTagType.Compound),
            };
        }

        [TestMethod]
        public void BlockEntitiesLoadAndSerialize()
        {
            var disk = new MemoryStream();
            foreach (var be in new[] { MakeBlockEntity("Chest", 3, -60, 5), MakeBlockEntity("Sign", 1, -44, 2) })
            {
                disk.Write(new fNbt.NbtFile(be) { BigEndian = false, UseVarInt = false }.SaveToBuffer(fNbt.NbtCompression.None));
            }

            var chunk = BuildChunk();
            chunk.LoadBlockEntities(disk.ToArray());
            Assert.AreEqual(2, chunk.GetBlockEntities().Count);
            Assert.AreEqual("Chest", chunk.GetBlockEntity(3, -60, 5)?.Get<fNbt.NbtString>("id")?.Value);

            //-60 >> 4 = -4 (sub chunk index 0), -44 >> 4 = -3 (index 1)
            Assert.AreEqual(1, chunk.GetBlockEntitiesInSubChunk(-4).Count);
            Assert.AreEqual(1, chunk.GetBlockEntitiesInSubChunk(-3).Count);

            byte[] withoutBe = new Chunk { Chunks = chunk.Chunks }.NetworkSerializeSubChunk(0, -4);
            byte[] withBe = chunk.NetworkSerializeSubChunk(0, -4);
            byte[] nbtBytes = withBe[withoutBe.Length..];

            var parsed = new fNbt.NbtFile { BigEndian = false, UseVarInt = true };
            long read = parsed.LoadFromBuffer(nbtBytes, 0, nbtBytes.Length, fNbt.NbtCompression.None);
            Assert.AreEqual(nbtBytes.Length, read);
            var root = (fNbt.NbtCompound)parsed.RootTag;
            Assert.AreEqual("Chest", root.Get<fNbt.NbtString>("id")?.Value);
            Assert.AreEqual(-60, root.Get<fNbt.NbtInt>("y")?.Value);
            Assert.IsFalse(root.Contains("Items")); //server only
            Assert.IsTrue(chunk.GetBlockEntity(3, -60, 5)!.Contains("Items")); //stored copy untouched

            Assert.IsTrue(chunk.RemoveBlockEntity(3, -60, 5));
            Assert.IsNull(chunk.GetBlockEntity(3, -60, 5));
            Assert.AreEqual(withoutBe.Length, chunk.NetworkSerializeSubChunk(0, -4).Length);
        }
    }
}

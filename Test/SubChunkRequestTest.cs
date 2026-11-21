using DaemonMC.Blocks;
using DaemonMC.Level;
using DaemonMC.Level.Generators;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using MiNET.LevelDB.Utils;

namespace Test
{
    [TestClass]
    public class SubChunkRequestTest
    {
        private static List<SubChunk> BuildSubChunks()
        {
            var subChunks = new List<SubChunk>();
            for (int i = 0; i < 3; i++)
            {
                subChunks.Add(ChunkUtils.CreateEmptySubChunk());
            }

            subChunks[0].Palette.Add(new Stone().GetState());
            Array.Fill(subChunks[0].Blocks, (byte)1);

            subChunks[1].Palette.Add(new Stone().GetState());
            for (int y = 0; y <= 4; y++)
            {
                subChunks[1].Blocks[(3 << 8) | (5 << 4) | y] = 1;
            }
            return subChunks;
        }

        private static int[] Heights(List<SubChunk> subChunks)
        {
            return ChunkColumn.ComputeHeights(subChunks.Count - 1, i => subChunks[i]);
        }

        [TestMethod]
        public void AirDetectionAndHighest()
        {
            var subChunks = BuildSubChunks();
            Assert.IsFalse(subChunks[0].IsAir());
            Assert.IsFalse(subChunks[1].IsAir());
            Assert.IsTrue(subChunks[2].IsAir());
            Assert.IsTrue(new SubChunk().IsAir());
            Assert.AreEqual(1, ChunkColumn.HighestNonAirSubChunk(Heights(subChunks)));
            Assert.AreEqual(-1, ChunkColumn.HighestNonAirSubChunk(ChunkColumn.ComputeHeights(-1, _ => null)));
        }

        [TestMethod]
        public void HeightsOnlyLoadWhatIsNeeded()
        {
            var subChunks = BuildSubChunks();
            var requested = new List<int>();
            ChunkColumn.ComputeHeights(2, i => { requested.Add(i); return subChunks[i]; });

            //sub chunk 0 is full, so every column is resolved there; nothing below would be touched
            CollectionAssert.AreEqual(new[] { 2, 1, 0 }, requested);

            requested.Clear();
            var full = ChunkUtils.CreateEmptySubChunk();
            full.Palette.Add(new Stone().GetState());
            Array.Fill(full.Blocks, (byte)1);
            ChunkColumn.ComputeHeights(5, i => { requested.Add(i); return i == 5 ? full : null; });
            CollectionAssert.AreEqual(new[] { 5 }, requested);
        }

        [TestMethod]
        public void EmptySubChunkDoesNotAllocateBlocks()
        {
            var subChunk = new SubChunk();
            Assert.IsTrue(subChunk.IsAir());
            Assert.IsNull(subChunk.GetBlockState(0, 0, 0));
            var field = typeof(SubChunk).GetField("blocks", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            Assert.IsNull(field.GetValue(subChunk));
        }

        [TestMethod]
        public void SetBlockState()
        {
            var subChunk = new SubChunk();
            subChunk.SetBlockState(3, 4, 5, new Stone().GetState());
            Assert.IsFalse(subChunk.IsAir());
            Assert.AreEqual("minecraft:stone", subChunk.GetBlockState(3, 4, 5)?["name"]?.StringValue);
            Assert.AreEqual("minecraft:air", subChunk.GetBlockState(0, 0, 0)?["name"]?.StringValue);

            subChunk.SetBlockState(3, 4, 5, new Air().GetState());
            Assert.IsTrue(subChunk.IsAir());
            Assert.AreEqual(2, subChunk.Palette.Count); //palette is not shrunk
        }

        [TestMethod]
        public void GeneratorPerSubChunk()
        {
            var generator = new SuperFlat();
            Assert.AreEqual(1, generator.SubChunkCount);

            var bottom = generator.GenerateSubChunk(0);
            Assert.AreEqual("minecraft:bedrock", bottom.GetBlockState(0, 0, 0)?["name"]?.StringValue);
            Assert.AreEqual("minecraft:grass_block", bottom.GetBlockState(15, 3, 15)?["name"]?.StringValue);
            Assert.AreEqual("minecraft:air", bottom.GetBlockState(0, 4, 0)?["name"]?.StringValue);

            Assert.IsTrue(generator.GenerateSubChunk(1).IsAir());
            Assert.IsTrue(generator.GenerateSubChunk(-1).IsAir());
        }

        [TestMethod]
        public void HeightMaps()
        {
            var heights = Heights(BuildSubChunks());
            Assert.AreEqual(16 + 4, heights[(5 << 4) | 3]);
            Assert.AreEqual(15, heights[0]);

            Assert.AreEqual(HeightMapDataType.HasData, ChunkColumn.BuildSubChunkHeightMap(heights, 0, out var hm0));
            Assert.AreEqual(16, hm0[(5 << 4) | 3]); //column continues above
            Assert.AreEqual(15, hm0[0]);

            Assert.AreEqual(HeightMapDataType.HasData, ChunkColumn.BuildSubChunkHeightMap(heights, 1, out var hm1));
            Assert.AreEqual(4, hm1[(5 << 4) | 3]);
            Assert.AreEqual(-1, hm1[0]);

            Assert.AreEqual(HeightMapDataType.AllTooLow, ChunkColumn.BuildSubChunkHeightMap(heights, 2, out _));

            var allHigh = Enumerable.Repeat(100, 256).ToArray();
            Assert.AreEqual(HeightMapDataType.AllTooHigh, ChunkColumn.BuildSubChunkHeightMap(allHigh, 0, out _));
        }

        [TestMethod]
        public void BiomePayload()
        {
            var column = new ChunkColumn(0, 0, Dimension.Overworld);
            column.Biomes[0] = new BiomeStorage { Palette = [1] };
            var data = column.NetworkSerializeBiomes();
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
            var data = BuildSubChunks()[1].NetworkSerialize(-3);
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

            var column = new ChunkColumn(0, 0, Dimension.Overworld);
            var subChunk = BuildSubChunks()[0];
            column.LoadBlockEntities(disk.ToArray());
            Assert.AreEqual(2, column.GetBlockEntities().Count);
            Assert.AreEqual("Chest", column.GetBlockEntity(3, -60, 5)?.Get<fNbt.NbtString>("id")?.Value);

            //-60 >> 4 = -4 (sub chunk index 0), -44 >> 4 = -3 (index 1)
            Assert.AreEqual(1, column.GetBlockEntitiesInSubChunk(-4).Count);
            Assert.AreEqual(1, column.GetBlockEntitiesInSubChunk(-3).Count);

            byte[] withoutBe = subChunk.NetworkSerialize(-4);
            byte[] withBe = subChunk.NetworkSerialize(-4, blockEntities: column.GetBlockEntitiesInSubChunk(-4));
            byte[] nbtBytes = withBe[withoutBe.Length..];

            var parsed = new fNbt.NbtFile { BigEndian = false, UseVarInt = true };
            long read = parsed.LoadFromBuffer(nbtBytes, 0, nbtBytes.Length, fNbt.NbtCompression.None);
            Assert.AreEqual(nbtBytes.Length, read);
            var root = (fNbt.NbtCompound)parsed.RootTag;
            Assert.AreEqual("Chest", root.Get<fNbt.NbtString>("id")?.Value);
            Assert.AreEqual(-60, root.Get<fNbt.NbtInt>("y")?.Value);
            Assert.IsFalse(root.Contains("Items")); //server only
            Assert.IsTrue(column.GetBlockEntity(3, -60, 5)!.Contains("Items")); //stored copy untouched

            Assert.IsTrue(column.RemoveBlockEntity(3, -60, 5));
            Assert.IsNull(column.GetBlockEntity(3, -60, 5));
            Assert.AreEqual(withoutBe.Length, subChunk.NetworkSerialize(-4, blockEntities: column.GetBlockEntitiesInSubChunk(-4)).Length);
        }
    }
}

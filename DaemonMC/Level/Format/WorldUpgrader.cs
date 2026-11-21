using System.Buffers.Binary;
using System.IO.Compression;
using System.Reflection;
using DaemonMC.Network;
using DaemonMC.Utils.Text;
using fNbt;
using MiNET.LevelDB;
using MiNET.LevelDB.Enumerate;
using MiNET.LevelDB.Utils;
using Version = System.Version;

namespace DaemonMC.Level.Format
{
    public static class WorldUpgrader
    {
        public const string BackupSuffix = "_backup";
        private const byte SubChunkPrefix = 0x2f;

        private static readonly Lazy<SortedList<Version, int>> KnownProtocols = new(() =>
        {
            var protocols = new SortedList<Version, int>();
            foreach (var field in typeof(Info).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(int) && field.Name.StartsWith('v') &&
                    Version.TryParse(field.Name[1..].Replace('_', '.'), out var version))
                {
                    protocols[version] = (int)field.GetValue(null)!;
                }
            }
            return protocols;
        });

        public static Version? GetWorldVersion(NbtCompound levelDat)
        {
            if (levelDat.TryGet<NbtList>("MinimumCompatibleClientVersion", out var list)&& list!.Count >= 3 && list.All(t => t is NbtInt))
            {
                return new Version(list[0].IntValue, list[1].IntValue, list[2].IntValue);
            }
            return null;
        }

        public static int ToProtocol(Version version)
        {
            int protocol = 0;
            foreach (var (known, knownProtocol) in KnownProtocols.Value)
            {
                if (known > version) break;
                protocol = knownProtocol;
            }
            return protocol;
        }

        public static bool Upgrade(string mcworldPath, int worldProtocol, string worldVersion)
        {
            string backupPath = GetBackupPath(mcworldPath);
            string stagingDir = Path.Combine(Path.GetTempPath(), $"DaemonMC-upgrade-{Guid.NewGuid():N}");
            string tempArchive = mcworldPath + ".upgrading";
            bool backupCreated = false;

            Log.warn($"World {mcworldPath} was saved with an older Minecraft version ({worldVersion}). Upgrading to {Info.Version}...");
            if (worldProtocol < StateConverter.OldestCoveredProtocol)
            {
                Log.warn($"Unsupported world version (protocol {StateConverter.OldestCoveredProtocol}). Blocks changed before that version won't be converted.");
            }

            try
            {
                File.Copy(mcworldPath, backupPath);
                backupCreated = true;
                Log.info($"Backup of the original world saved to {backupPath}");

                ZipFile.ExtractToDirectory(mcworldPath, stagingDir);

                string levelDat = Directory.GetFiles(stagingDir, "level.dat", SearchOption.AllDirectories).FirstOrDefault()
                    ?? throw new FileNotFoundException("level.dat not found in world archive");
                string dbDir = Path.Combine(Path.GetDirectoryName(levelDat)!, "db");

                var (upgraded, total, skipped) = UpgradeDatabase(dbDir, worldProtocol);
                UpgradeLevelDat(levelDat);

                if (File.Exists(tempArchive))
                {
                    File.Delete(tempArchive);
                }
                ZipFile.CreateFromDirectory(stagingDir, tempArchive);
                File.Move(tempArchive, mcworldPath, overwrite: true);

                Log.info($"World {mcworldPath} upgraded to {Info.Version}. Converted {upgraded} of {total} sub chunks.");
                if (skipped > 0)
                {
                    Log.warn($"{skipped} sub chunks use an unsupported storage format and were left unchanged.");
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.error($"Failed to upgrade world {mcworldPath}: {ex}");
                if (backupCreated)
                {
                    TryDelete(() => File.Delete(backupPath));
                }
                Log.warn($"World {mcworldPath} was left unchanged. Open it with Minecraft client {World.LevelDbVersion.Last()} and export the mcworld file again to update it manually.");
                return false;
            }
            finally
            {
                TryDelete(() => File.Delete(tempArchive));
                TryDelete(() => Directory.Delete(stagingDir, true));
            }
        }

        private static string GetBackupPath(string mcworldPath)
        {
            string backupPath = mcworldPath + BackupSuffix;
            if (File.Exists(backupPath))
            {
                backupPath = $"{mcworldPath}{BackupSuffix}_{DateTime.Now:yyyyMMdd_HHmmss}";
            }
            return backupPath;
        }

        private static (int upgraded, int total, int skipped) UpgradeDatabase(string dbDir, int worldProtocol)
        {
           
            List<byte[]> keys = CollectSubChunkKeys(dbDir);
            int upgraded = 0, total = 0, skipped = 0;

            using var db = new Database(new DirectoryInfo(dbDir));
            db.Open();

            // upgrade every palette entry
            var needsNeighbors = new List<(byte[] key, HashSet<int> paletteIndices)>();
            foreach (byte[] key in keys)
            {
                byte[] data = db.Get(key);
                if (data == null)
                {
                    continue;
                }

                total++;
                PersistedSubChunk subChunk;
                try
                {
                    subChunk = PersistedSubChunk.Parse(data);
                }
                catch (Exception ex)
                {
                    skipped++;
                    Log.debug($"Skipped sub chunk {Convert.ToHexString(key)}: {ex.Message}");
                    continue;
                }

                HashSet<int>? connected = null;
                for (int l = 0; l < subChunk.Layers.Count; l++)
                {
                    var layer = subChunk.Layers[l];
                    for (int i = 0; i < layer.Palette.Count; i++)
                    {
                        var oldState = layer.Palette[i];
                        var newState = StateConverter.Upgrade(oldState, worldProtocol);
                        if (ReferenceEquals(oldState, newState))
                        {
                            continue;
                        }

                        layer.Palette[i] = newState;
                        layer.Dirty = true;

                        if (l == 0 && GainedComputedState(oldState, newState))
                        {
                            (connected ??= new HashSet<int>()).Add(i);
                        }
                    }
                }

                if (subChunk.Layers.Any(l => l.Dirty))
                {
                    db.Put(key, subChunk.Serialize());
                    upgraded++;
                }
                if (connected != null)
                {
                    needsNeighbors.Add((key, connected));
                }
            }

            // compute connections from already upgraded neighbouring blocks
            var world = new BlockReader(db);
            int resolvedBlocks = 0;
            foreach (var (key, paletteIndices) in needsNeighbors)
            {
                resolvedBlocks += ResolveConnections(db, world, key, paletteIndices);
            }
            if (resolvedBlocks > 0)
            {
                Log.debug($"Computed connections for {resolvedBlocks} blocks");
            }

            db.Close();
            return (upgraded, total, skipped);
        }

        private static bool GainedComputedState(NbtCompound oldState, NbtCompound newState)
        {
            if (!newState.TryGet<NbtCompound>("states", out var newStates))
            {
                return false;
            }
            oldState.TryGet<NbtCompound>("states", out var oldStates);
            return newStates!.Names.Any(n => ConnectionResolver.ComputedStates.Contains(n) && (oldStates == null || !oldStates.Contains(n)));
        }

        private static int ResolveConnections(Database db, BlockReader world, byte[] key, HashSet<int> paletteIndices)
        {
            var (dimension, chunkX, chunkZ, subChunkY) = ParseSubChunkKey(key);
            var subChunk = PersistedSubChunk.Parse(db.Get(key));
            var layer = subChunk.Layers[0];
            var resolved = new Dictionary<(int index, int hash), int>();
            int count = 0;

            for (int i = 0; i < 4096; i++)
            {
                int paletteIndex = layer.Blocks[i];
                if (!paletteIndices.Contains(paletteIndex))
                {
                    continue;
                }

                int x = chunkX * 16 + (i >> 8);
                int z = chunkZ * 16 + ((i >> 4) & 15);
                int y = subChunkY * 16 + (i & 15);

                var original = layer.Palette[paletteIndex];
                var state = new StateConverter.BlockState(original);
                if (!ConnectionResolver.Resolve(state, (dx, dy, dz) => world.Get(dimension, x + dx, y + dy, z + dz)))
                {
                    continue;
                }

                var newState = StateConverter.Rebuild(state, original);
                int hash = StateConverter.Hash(newState);
                if (!resolved.TryGetValue((paletteIndex, hash), out int newIndex))
                {
                    newIndex = layer.IndexOf(newState);
                    if (newIndex == -1)
                    {
                        layer.Palette.Add(newState);
                        newIndex = layer.Palette.Count - 1;
                    }
                    resolved[(paletteIndex, hash)] = newIndex;
                }

                layer.Blocks[i] = (ushort)newIndex;
                layer.Dirty = true;
                count++;
            }

            if (layer.Dirty)
            {
                db.Put(key, subChunk.Serialize());
            }
            return count;
        }

        private static (int dimension, int x, int z, int y) ParseSubChunkKey(byte[] key)
        {
            int x = BinaryPrimitives.ReadInt32LittleEndian(key.AsSpan(0, 4));
            int z = BinaryPrimitives.ReadInt32LittleEndian(key.AsSpan(4, 4));
            int dimension = key.Length == 14 ? BinaryPrimitives.ReadInt32LittleEndian(key.AsSpan(8, 4)) : 0;
            return (dimension, x, z, (sbyte)key[^1]);
        }

        private static byte[] SubChunkKey(int dimension, int x, int z, int y)
        {
            var key = new byte[dimension == 0 ? 10 : 14];
            BinaryPrimitives.WriteInt32LittleEndian(key.AsSpan(0, 4), x);
            BinaryPrimitives.WriteInt32LittleEndian(key.AsSpan(4, 4), z);
            if (dimension != 0)
            {
                BinaryPrimitives.WriteInt32LittleEndian(key.AsSpan(8, 4), dimension);
            }
            key[^2] = SubChunkPrefix;
            key[^1] = (byte)(sbyte)y;
            return key;
        }

        private sealed class BlockReader(Database db)
        {
            private const int MaxCachedSubChunks = 4096;
            private readonly Dictionary<(int, int, int, int), PersistedSubChunk.Layer?> cache = new();

            public NbtCompound? Get(int dimension, int x, int y, int z)
            {
                int subChunkY = y >> 4;
                if (subChunkY < sbyte.MinValue || subChunkY > sbyte.MaxValue)
                {
                    return null;
                }

                var position = (dimension, x >> 4, z >> 4, subChunkY);
                if (!cache.TryGetValue(position, out var layer))
                {
                    if (cache.Count >= MaxCachedSubChunks)
                    {
                        cache.Clear();
                    }

                    byte[] data = db.Get(SubChunkKey(dimension, x >> 4, z >> 4, subChunkY));
                    try
                    {
                        layer = data == null ? null : PersistedSubChunk.Parse(data).Layers.FirstOrDefault();
                    }
                    catch (Exception)
                    {
                        layer = null;
                    }
                    cache[position] = layer;
                }

                if (layer == null)
                {
                    return null;
                }

                int index = layer.Blocks[((x & 15) << 8) | ((z & 15) << 4) | (y & 15)];
                return index < layer.Palette.Count ? layer.Palette[index] : null;
            }
        }

        private static void UpgradeLevelDat(string levelDatPath)
        {
            byte[] raw = File.ReadAllBytes(levelDatPath);
            int storageVersion = BinaryPrimitives.ReadInt32LittleEndian(raw);

            var nbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false
            };
            nbt.LoadFromBuffer(raw, 8, raw.Length - 8, NbtCompression.None);
            var root = (NbtCompound)nbt.RootTag;

            int[] version = WorldCreator.ToVersionArray(World.LevelDbVersion.Last());
            Replace(root, new NbtInt("NetworkVersion", Info.ProtocolVersion.Last()));
            Replace(root, new NbtList("lastOpenedWithVersion", version.Select(v => new NbtInt(v)), NbtTagType.Int));
            Replace(root, new NbtList("MinimumCompatibleClientVersion", version.Select(v => new NbtInt(v)), NbtTagType.Int));

            byte[] body = nbt.SaveToBuffer(NbtCompression.None);
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write(storageVersion);
            writer.Write(body.Length);
            writer.Write(body);
            File.WriteAllBytes(levelDatPath, stream.ToArray());
        }

        private static void Replace(NbtCompound root, NbtTag tag)
        {
            root.Remove(tag.Name!);
            root.Add(tag);
        }

        private static List<byte[]> CollectSubChunkKeys(string dbDir)
        {
            var seen = new HashSet<string>();
            var keys = new List<byte[]>();

            void Add(ReadOnlySpan<byte> key)
            {
                if (IsSubChunkKey(key) && seen.Add(Convert.ToHexString(key)))
                {
                    keys.Add(key.ToArray());
                }
            }

            var dir = new DirectoryInfo(dbDir);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"World database not found: {dbDir}");
            }

            MethodInfo initialize = typeof(Table).GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new MissingMethodException(nameof(Table), "Initialize");

            foreach (var file in dir.GetFiles("*.ldb").Concat(dir.GetFiles("*.sst")))
            {
                using var table = new Table(file);
                initialize.Invoke(table, null);
                var enumerator = new TableEnumerator(table);


                for (var entry = enumerator.Current; entry != null; entry = enumerator.MoveNext() ? enumerator.Current : null)
                {
                    if (entry.Key.Length > 8)
                    {
                        Add(entry.Key.Span[..^8]);
                    }
                }
            }

            foreach (var file in dir.GetFiles("*.log"))
            {
                using var reader = new LogReader(file);
                while (true)
                {
                    ReadOnlySpan<byte> batch = reader.ReadData();
                    if (reader.Eof)
                    {
                        break;
                    }

                    var batchReader = new SpanReader(batch);
                    batchReader.ReadUInt64(); // sequence
                    uint count = batchReader.ReadUInt32();
                    for (int i = 0; i < count; i++)
                    {
                        byte operation = batchReader.ReadByte();
                        Add(batchReader.ReadLengthPrefixedBytes());
                        if (operation == 1)
                        {
                            batchReader.ReadLengthPrefixedBytes(); // value
                        }
                    }
                }
            }

            return keys;
        }

        // Sub chunk keys x(4) z(4) [dimension(4)] 0x2f y(1)
        private static bool IsSubChunkKey(ReadOnlySpan<byte> key)
        {
            return key.Length switch
            {
                10 => key[8] == SubChunkPrefix,
                14 => key[12] == SubChunkPrefix && BinaryPrimitives.ReadInt32LittleEndian(key.Slice(8, 4)) is 1 or 2,
                _ => false
            };
        }

        private static void TryDelete(Action delete)
        {
            try
            {
                delete();
            }
            catch (DirectoryNotFoundException) { }
            catch (Exception ex)
            {
                Log.debug($"Couldn't clean up after world upgrade: {ex.Message}");
            }
        }
    }
}

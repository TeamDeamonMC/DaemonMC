using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;

namespace DaemonMC.Utils
{
    public class PlayerUtils
    {
        internal readonly Player p;
        internal Queue<(int x, int z)> ChunkSendQueue = new Queue<(int x, int z)>();
        internal HashSet<(int x, int z)> sentChunks = new();
        internal bool SendQueueBusy = false;

        public PlayerUtils(Player player)
        {
            p = player;
        }

        internal void SendStartGame()
        {
            var packet = new StartGame
            {
                LevelName = p.CurrentWorld.LevelDisplayName,
                EntityId = p.EntityID,
                GameType = p.GameMode,
                GameMode = p.GameMode,
                Position = new Vector3(p.Position.X, p.Position.Y, p.Position.Z),
                Rotation = new Vector2(0, 0),
                SpawnBlockX = (int)p.Position.X,
                SpawnBlockY = (int)p.Position.Y,
                SpawnBlockZ = (int)p.Position.Z,
                Difficulty = 1,
                Dimension = (int)p.CurrentWorld.Dimension,
                Seed = p.CurrentWorld.RandomSeed,
                Generator = 1,
            };
            p.Send(packet);
        }

        internal async void ProcessSendQueue()
        {
            if (SendQueueBusy) { return; }
            SendQueueBusy = true;
            while (true)
            {
                (int x, int z) next;
                lock (ChunkSendQueue)
                {
                    if (ChunkSendQueue.Count == 0) { break; }
                    if (!Server.OnlinePlayers.ContainsValue(p))
                    {
                        ChunkSendQueue.Clear();
                        break;
                    }
                    next = ChunkSendQueue.Dequeue();
                }
                SendChunkToPlayer(next.x, next.z);

                await Task.Delay(10);
            }
            SendQueueBusy = false;
        }

        internal void SendChunks()
        {
            var packet2 = new NetworkChunkPublisherUpdate
            {
                X = (int)p.Position.X,
                Y = (int)p.Position.Y,
                Z = (int)p.Position.Z,
                Radius = p.drawDistance
            };
            p.Send(packet2);

            int radius = p.drawDistance / 2;

            int currentChunkX = (int)Math.Floor(p.Position.X / 16.0);
            int currentChunkZ = (int)Math.Floor(p.Position.Z / 16.0);

            List<(int x, int z)> chunkPositions = ChunkUtils.GetSequence(radius, currentChunkX, currentChunkZ);
            var inRange = chunkPositions.ToHashSet();

            lock (ChunkSendQueue)
            {
                var queued = ChunkSendQueue.ToHashSet();
                ChunkSendQueue.Clear();
                sentChunks.RemoveWhere(c => !inRange.Contains(c));

                foreach (var pos in chunkPositions)
                {
                    if (queued.Contains(pos) || sentChunks.Add(pos))
                    {
                        ChunkSendQueue.Enqueue(pos);
                    }
                }
            }

            ProcessSendQueue();
        }

        internal bool ClientHasChunk(int chunkX, int chunkZ)
        {
            lock (ChunkSendQueue)
            {
                return sentChunks.Contains((chunkX, chunkZ)) && !ChunkSendQueue.Contains((chunkX, chunkZ));
            }
        }

        private (int x, int y, int z) arrivalTarget;
        private TaskCompletionSource<bool>? arrival;

        internal async Task<bool> WaitForSubChunk(int chunkX, int subChunkY, int chunkZ, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (ChunkSendQueue)
            {
                arrival?.TrySetResult(false);
                arrival = tcs;
                arrivalTarget = (chunkX, subChunkY, chunkZ);
            }

            var finished = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
            lock (ChunkSendQueue)
            {
                if (arrival == tcs) { arrival = null; }
            }
            return finished == tcs.Task && tcs.Task.Result;
        }

        internal void ClearChunkCache()
        {
            lock (ChunkSendQueue)
            {
                ChunkSendQueue.Clear();
                sentChunks.Clear();
            }
            SendQueueBusy = false;
        }

        internal void ResendChunk(int chunkX, int chunkZ)
        {
            if (ClientHasChunk(chunkX, chunkZ))
            {
                SendChunkToPlayer(chunkX, chunkZ);
            }
        }

        internal void ResendChunks()
        {
            List<(int x, int z)> chunks;
            lock (ChunkSendQueue)
            {
                chunks = sentChunks.Where(c => !ChunkSendQueue.Contains(c)).ToList();
            }
            foreach (var (x, z) in chunks)
            {
                SendChunkToPlayer(x, z);
            }
        }

        internal void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            var world = p.CurrentWorld;
            var column = world.GetColumn(chunkX, chunkZ);
            int limit = Math.Max(world.GetHighestNonAirSubChunk(chunkX, chunkZ) + 1, 1);

            p.Send(new LevelChunk
            {
                ChunkX = chunkX,
                ChunkZ = chunkZ,
                Dimension = (int)world.Dimension,
                SubChunkLimit = limit,
                Data = column.NetworkSerializeBiomes(world.BiomeOverride)
            });
        }

        internal void HandleSubChunkRequest(SubChunkRequest request)
        {
            var world = p.CurrentWorld;
            int minIndex = world.MinY >> 4;
            int subChunkCount = world.Dimension.SubChunkCount();
            bool wrongDimension = request.Dimension != (int)world.Dimension;

            var response = new SubChunkPacket
            {
                Dimension = request.Dimension,
                CenterX = request.CenterX,
                CenterY = request.CenterY,
                CenterZ = request.CenterZ,
            };

            int protocol = RakSessionManager.getSession(p.ep).protocolVersion;

            foreach (var (ox, oy, oz) in request.Offsets)
            {
                var entry = new SubChunkEntry { OffsetX = ox, OffsetY = oy, OffsetZ = oz };
                response.Entries.Add(entry);

                int chunkX = request.CenterX + ox;
                int chunkZ = request.CenterZ + oz;
                int index = request.CenterY + oy - minIndex;

                if (wrongDimension)
                {
                    entry.Result = SubChunkRequestResult.WrongDimension;
                    continue;
                }
                if (index < 0 || index >= subChunkCount)
                {
                    entry.Result = SubChunkRequestResult.IndexOutOfBounds;
                    continue;
                }
                bool sent;
                lock (ChunkSendQueue) { sent = sentChunks.Contains((chunkX, chunkZ)); }
                if (!sent)
                {
                    entry.Result = SubChunkRequestResult.LevelChunkDoesntExist;
                    continue;
                }

                int subChunkY = request.CenterY + oy;
                var heights = world.GetColumnHeights(chunkX, chunkZ);

                entry.HeightMapType = ChunkColumn.BuildSubChunkHeightMap(heights, index, out var heightMap);
                entry.HeightMap = heightMap;
                entry.RenderHeightMapType = entry.HeightMapType;
                entry.RenderHeightMap = heightMap;

                var subChunk = world.GetSubChunk(chunkX, subChunkY, chunkZ)!;
                if (subChunk.IsAir())
                {
                    entry.Result = SubChunkRequestResult.SuccessAllAir;
                }
                else
                {
                    var blockEntities = world.GetColumn(chunkX, chunkZ).GetBlockEntitiesInSubChunk(subChunkY);
                    entry.Result = SubChunkRequestResult.Success;
                    entry.Payload = subChunk.NetworkSerialize(subChunkY, protocol, blockEntities);
                }
            }

            p.Send(response);

            lock (ChunkSendQueue)
            {
                if (arrival != null && !wrongDimension && response.Entries.Exists(e => (request.CenterX + e.OffsetX, request.CenterY + e.OffsetY, request.CenterZ + e.OffsetZ) == arrivalTarget && (e.Result == SubChunkRequestResult.Success || e.Result == SubChunkRequestResult.SuccessAllAir)))
                {
                    arrival.TrySetResult(true);
                }
            }
        }

        internal void UpdateFlags(List<AuthInputData> flags)
        {
            long data = p.dataValue;
            if (flags.Contains(AuthInputData.Sneaking))
            {
                p.SetFlag(ActorFlags.SNEAKING, true);
            }
            if (flags.Contains(AuthInputData.StopSneaking))
            {
                p.SetFlag(ActorFlags.SNEAKING, false);
            }
            if (flags.Contains(AuthInputData.StartSprinting))
            {
                p.SetFlag(ActorFlags.SPRINTING, true);
            }
            if (flags.Contains(AuthInputData.StopSprinting))
            {
                p.SetFlag(ActorFlags.SPRINTING, false);
            }
            if (data != p.dataValue)
            {
                p.SendMetadata(true);
            }
        }
    }
}

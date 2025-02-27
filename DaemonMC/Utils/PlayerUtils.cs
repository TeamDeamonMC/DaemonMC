using System.Numerics;
using DaemonMC.Level.Generators;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;

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
                Dimension = 0,
                Seed = p.CurrentWorld.RandomSeed,
                Generator = 1,
            };
            p.Send(packet);
        }

        internal async void ProcessSendQueue()
        {
            if (SendQueueBusy) { return; }
            SendQueueBusy = true;
            while (ChunkSendQueue.Count > 0)
            {
                if (!Server.OnlinePlayers.ContainsValue(p))
                {
                    ChunkSendQueue.Clear();
                    break;
                }
                var (chunkX, chunkZ) = ChunkSendQueue.Dequeue();
                SendChunkToPlayer(chunkX, chunkZ);

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

            foreach (var (x, z) in chunkPositions)
            {
                if (!sentChunks.Contains((x, z)))
                {
                    sentChunks.Add((x, z));
                    ChunkSendQueue.Enqueue((x, z));
                }
            }

            ProcessSendQueue();
        }

        internal void ClearChunkCache()
        {
            ChunkSendQueue.Clear();
            sentChunks.Clear();
            SendQueueBusy = false;
        }

        internal void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            List<byte> chunkData = new List<byte>();
            int chunkCount = 0;

            if (p.CurrentWorld.Temporary)
            {
                chunkData = new List<byte>(new SuperFlat().generateChunks());
                chunkCount = 20;
            }
            else
            {
                var chunkRaw = p.CurrentWorld.GetChunk(chunkX, chunkZ);
                chunkData = new List<byte>(chunkRaw.NetworkSerialize(p));
                chunkCount = chunkRaw.Chunks.Count();
                if (chunkRaw.Chunks.Count == 0)
                {
                    chunkData = new List<byte>(new SuperFlat().generateChunks());
                    chunkCount = 20;
                }
            }

            var chunk = new LevelChunk
            {
                ChunkX = chunkX,
                ChunkZ = chunkZ,
                Count = chunkCount,
                Data = chunkData.ToArray()
            };
            p.Send(chunk);
        }

        internal void UpdateFlags(List<AuthInputData> flags)
        {
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
            p.SendMetadata(true);
        }
    }
}

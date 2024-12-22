using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Utils.Text;
using fNbt;
using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Utils;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;

namespace DaemonMC
{
    public class Player
    {
        public string Username { get; set; }
        public long EntityID { get; set; }
        public Vector3 Position { get; set; }
        public int drawDistance { get; set; }
        public IPEndPoint ep { get; set; }
        public World currentLevel { get; set; }
        public AttributesValues attributes { get; set; } = new AttributesValues(0.1f);
        public Dictionary<ActorData, Metadata> metadata { get; set; } = new Dictionary<ActorData, Metadata>();

        private Queue<(int x, int z)> ChunkSendQueue = new Queue<(int x, int z)>();
        private bool SendQueueBusy = false;
        private int LastChunkX = 0;
        private int LastChunkZ = 0;

        public void spawn()
        {
            SendStartGame();
            SendCreativeInventory();
            SendBiomeDefinitionList();
            SendPlayStatus(3);
            UpdateAttributes();
            SendMetadata();
            Log.info($"{Username} spawned at X:{Position.X} Y:{Position.Y} Z:{Position.Z}");
        }

        public void SendStartGame()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new StartGame
            {
                LevelName = currentLevel.LevelDisplayName,
                EntityId = EntityID,
                GameType = 0,
                GameMode = 2,
                Position = new Vector3(currentLevel.spawnX, 5, currentLevel.spawnZ),
                Rotation = new Vector2(0, 0),
                SpawnBlockX = currentLevel.spawnX,
                SpawnBlockY = 0,
                SpawnBlockZ = currentLevel.spawnZ,
                Difficulty = 1,
                Dimension = 0,
                Seed = currentLevel.RandomSeed,
                Generator = 1,
            };
            packet.Encode(encoder);
        }

        public void SendCreativeInventory()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new CreativeContent
            {

            };
            packet.Encode(encoder);
        }

        public void SendBiomeDefinitionList()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new BiomeDefinitionList
            {
                biomeData = new fNbt.NbtCompound("")
                {
                new NbtCompound("plains")
                    {
                        new NbtFloat("downfall", 0.4f),
                        new NbtFloat("temperature", 0.8f),
                    }
                }
            };
            packet.Encode(encoder);
        }

        public void SendPlayStatus(int status)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new PlayStatus
            {
                status = status,
            };
            packet.Encode(encoder);
        }

        public void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var chunk = new LevelChunk
            {
                chunkX = chunkX,
                chunkZ = chunkZ,
                count = 20,
                data = currentLevel.temporary ? testchunk.generateChunks() : new LevelDBInterface().GetChunk(currentLevel.levelName, chunkX, chunkZ).networkSerialize(this)
            };
            chunk.Encode(encoder);
        }

        public void UpdateChunkRadius(int radius)
        {
            drawDistance = radius;
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new ChunkRadiusUpdated
            {
                radius = drawDistance,
            };
            packet.Encode(encoder);
        }

        public void UpdateAttributes()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new UpdateAttributes
            {
                EntityId = EntityID,
                Attributes = new List<AttributeValue> { attributes.Movement_speed() }
            };
            packet.Encode(encoder);
        }

        public void SendMetadata()
        {
            metadata[ActorData.RESERVED_0] = new Metadata(844459290443776); //todo add dataflags

            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new SetActorData
            {
                EntityId = EntityID,
                Metadata = metadata
            };
            packet.Encode(encoder);
        }

        private async void ProcessSendQueue()
        {
            if (SendQueueBusy) { return; }
            SendQueueBusy = true;
            while (ChunkSendQueue.Count > 0)
            {
                if (!Server.onlinePlayers.ContainsValue(this)) {
                    ChunkSendQueue.Clear();
                    break;
                }
                var (chunkX, chunkZ) = ChunkSendQueue.Dequeue();
                SendChunkToPlayer(chunkX, chunkZ);

                await Task.Delay(50);
            }
            SendQueueBusy = false;
        }

        public void Kick(string msg)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new Disconnect
            {
                message = msg
            };
            packet.Encode(encoder);
            Server.RemovePlayer((long)EntityID);
            RakSessionManager.deleteSession(ep);
        }

        public void SendMessage(string message)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var pk = new TextMessage
            {
                messageType = 1,
                Message = message
            };
            pk.Encode(encoder);
        }



        //
        //Packet processors for spawned player
        //



        public void PacketEvent_PlayerAuthInput(PlayerAuthInput packet)
        {
            if (Position != packet.Position)
            {
                Position = packet.Position;
                int currentChunkX = (int)Math.Floor(packet.Position.X / 16.0);
                int currentChunkZ = (int)Math.Floor(packet.Position.Z / 16.0);

                Log.debug($"{packet.Position.X} : {packet.Position.Y} : {packet.Position.Z} Data: {string.Join(" | ", packet.InputData)}");
            }
        }

        public void PacketEvent_MovePlayer(MovePlayer packet)
        {
            Position = packet.position;

            int currentChunkX = (int)Math.Floor(packet.position.X / 16.0);
            int currentChunkZ = (int)Math.Floor(packet.position.Z / 16.0);

            //Log.debug($"{packet.actorRuntimeId} / {packet.position.X} : {packet.position.Y} : {packet.position.Z}");
        }

        public void PacketEvent_RequestChunkRadius(RequestChunkRadius packet)
        {
            Log.debug($"{Username} requested chunks with radius {packet.radius}. Max radius = {packet.maxRadius}");

            UpdateChunkRadius(packet.radius);

            PacketEncoder encoder2 = PacketEncoderPool.Get(this);
            var packet2 = new NetworkChunkPublisherUpdate
            {
                x = (int) Position.X,
                y = (int) Position.Y,
                z = (int) Position.Z,
                radius = drawDistance
            };
            packet2.Encode(encoder2);

            int radius = Math.Min(packet.radius, packet.maxRadius) / 2;

            int currentChunkX = (int)Math.Floor(Position.X / 16.0);
            int currentChunkZ = (int)Math.Floor(Position.Z / 16.0);

            List<(int x, int z)> chunkPositions = ChunkUtils.GetSequence(radius, currentChunkX, currentChunkZ);

            foreach (var (x, z) in chunkPositions)
            {
                ChunkSendQueue.Enqueue((x, z));
            }

            ProcessSendQueue();
        }

        public void PacketEvent_Text(TextMessage packet)
        {
            foreach (var dest in Server.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var pk = new TextMessage
                {
                    messageType = 1,
                    Username = Username,
                    Message = packet.Message
                };
                pk.Encode(encoder);
            }
        }
    }
}

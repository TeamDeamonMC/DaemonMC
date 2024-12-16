using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Utils.Text;
using fNbt;
using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Utils;

namespace DaemonMC
{
    public class Player
    {
        public string Username { get; set; }
        public long EntityID { get; set; }
        public Vector3 Position { get; set; } = new Vector3(0, 5, 0);
        public int drawDistance { get; set; }
        public IPEndPoint ep { get; set; }
        public Level.Level currentLevel { get; set; }

        public void spawn()
        {
            SendStartGame();
            SendCreativeInventory();
            SendBiomeDefinitionList();
            SendPlayStatus(3);
            Log.info($"{Username} spawned at X:{Position.X} Y:{Position.Y} Z:{Position.Z}");
        }

        public void SendStartGame()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new StartGame
            {
                EntityId = EntityID,
                gameType = 0,
                GameMode = 2,
                position = Position,
                rotation = new Vector2(0, 0),
                spawnBlockX = 0,
                spawnBlockY = 0,
                spawnBlockZ = 0,
                difficulty = 1,
                dimension = 0,
                seed = 9876,
                generator = 1,
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

        private void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var chunk = new LevelChunk
            {
                chunkX = chunkX,
                chunkZ = chunkZ,
                count = 20,
                data = testchunk.flat
            };
            chunk.Encode(encoder);
        }

        private void UpdateChunkRadius(int radius)
        {
            drawDistance = radius;
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new ChunkRadiusUpdated
            {
                radius = drawDistance,
            };
            packet.Encode(encoder);
        }

        //Packet processors for spawned player

        public void PacketEvent_MovePlayer(MovePlayer packet)
        {
            Position = packet.position;

            int currentChunkX = (int)Math.Floor(packet.position.X / 16.0);
            int currentChunkZ = (int)Math.Floor(packet.position.Z / 16.0);

            //Log.debug($"{packet.actorRuntimeId} / {packet.position.X} : {packet.position.Y} : {packet.position.Z}");
        }

        public async Task PacketEvent_RequestChunkRadius(RequestChunkRadius packet)
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

            int radius = Math.Min(packet.radius, packet.maxRadius) / 4;

            List<(int x, int z)> chunkPositions = ChunkUtils.GetSequence(radius, (int) Position.X, (int) Position.Z);

            foreach (var (x, z) in chunkPositions)
            {
                SendChunkToPlayer(x, z);

                await Task.Delay(20);
            }
        }

        public void PacketEvent_Text(TextMessage packet)
        {
            foreach (var dest in currentLevel.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var pk = new TextMessage
                {
                    Username = Username,
                    Message = packet.Message
                };
                pk.Encode(encoder);
            }
        }
    }
}

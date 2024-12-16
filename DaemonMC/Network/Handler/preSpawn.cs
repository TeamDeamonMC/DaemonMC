using System.Net;
using System.Numerics;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;
using fNbt;

namespace DaemonMC.Network.Handler
{
    public class preSpawn
    {
        public static void execute(IPEndPoint clientEp)
        {
            var session = RakSessionManager.getSession(clientEp);

            Player player = new Player();
            player.username = session.username;

            long EntityId = Server.AddPlayer(player, clientEp);
            session.EntityID = EntityId;

            PacketEncoder encoder1 = PacketEncoderPool.Get(clientEp);
            var pk1 = new StartGame
            {
                EntityId = EntityId,
                gameType = 0,
                GameMode = 2,
                position = new Vector3(0, 5, 0),
                rotation = new Vector2(0, 0),
                spawnBlockX = 0,
                spawnBlockY = 0,
                spawnBlockZ = 0,
                difficulty = 1,
                dimension = 0,
                seed = 9876,
                generator = 1,
            };
            pk1.Encode(encoder1);

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new CreativeContent
            {

            };
            pk.Encode(encoder);

            PacketEncoder encoder2 = PacketEncoderPool.Get(clientEp);
            var pk2 = new BiomeDefinitionList
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
            pk2.Encode(encoder2);

            PacketEncoder encoder4 = PacketEncoderPool.Get(clientEp);
            var pk4 = new PlayStatus
            {
                status = 3,
            };
            pk4.Encode(encoder4);
            Log.info($"{player.username} spawned at X:{pk1.position.X} Y:{pk1.position.Y} Z:{pk1.position.Z}");
        }
    }
}

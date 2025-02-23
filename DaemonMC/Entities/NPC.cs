using DaemonMC.Level;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Utils;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Entities
{
    public class CustomEntity : Entity
    {
        public Guid UUID { get; set; } = new Guid();
        public Skin Skin { get; set; } = new Skin();

        public override void Spawn(World world)
        {
            currentWorld = world;
            long id;

            if (Server.availableIds.Count > 0)
            {
                id = Server.availableIds.Dequeue();
            }
            else
            {
                id = Server.nextId++;
            }

            EntityId = id;

            world.Entities.Add(id, this);

            Metadata[ActorData.NAMETAG_ALWAYS_SHOW] = new Metadata((byte)1);
            if (NameTag != "") { Metadata[ActorData.NAME] = new Metadata(NameTag); }

            foreach (Player onlinePlayer in currentWorld.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(onlinePlayer);
                var packet = new AddPlayer
                {
                    UUID = UUID,
                    Username = NameTag,
                    EntityId = EntityId,
                    Position = Position,
                    Metadata = Metadata
                };
                packet.EncodePacket(encoder);

                PacketEncoder encoder2 = PacketEncoderPool.Get(onlinePlayer);
                var packet2 = new PlayerList
                {
                    UUID = UUID,
                    EntityId = EntityId,
                    Username = NameTag,
                    Skin = Skin
                };
                packet2.EncodePacket(encoder2);
            }

            _ = Task.Run(async () => {
                await Task.Delay(2000);
                foreach (Player onlinePlayer in currentWorld.onlinePlayers.Values)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(onlinePlayer);
                    var packet = new PlayerList
                    {
                        action = 1,
                        UUID = UUID,
                    };
                    packet.EncodePacket(encoder);
                }

                if (ResourcePackManager.Animations.TryGetValue(SpawnAnimation, out Animation spawnAnimation))
                {
                    foreach (Player onlinePlayer in currentWorld.onlinePlayers.Values)
                    {
                        PacketEncoder encoder = PacketEncoderPool.Get(onlinePlayer);
                        var packet1 = new AnimateEntity
                        {
                            mAnimation = spawnAnimation.AnimationName,
                            mController = spawnAnimation.ControllerName,
                            mRuntimeId = EntityId
                        };
                        packet1.EncodePacket(encoder);
                    }
                }
                else
                {
                    Log.warn($"Unable to find spawn animation by key:{SpawnAnimation}. Make sure animation is registered.");
                }
            });
        }
    }
}

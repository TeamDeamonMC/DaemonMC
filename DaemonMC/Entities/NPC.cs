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
            CurrentWorld = world;
            long id;

            if (Server.AvailableIds.Count > 0)
            {
                id = Server.AvailableIds.Dequeue();
            }
            else
            {
                id = Server.NextId++;
            }

            EntityId = id;

            world.Entities.Add(id, this);

            Metadata[ActorData.NAMETAG_ALWAYS_SHOW] = new Metadata((byte)1);
            if (NameTag != "") { Metadata[ActorData.NAME] = new Metadata(NameTag); }

            var packet = new AddPlayer
            {
                UUID = UUID,
                Username = NameTag,
                EntityId = EntityId,
                Position = Position,
                Metadata = Metadata
            };
            CurrentWorld.Send(packet);

            var packet2 = new PlayerList
            {
                UUID = UUID,
                EntityId = EntityId,
                Username = NameTag,
                Skin = Skin
            };
            CurrentWorld.Send(packet2);

            _ = Task.Run(async () => {
                await Task.Delay(2000);
                var packet3 = new PlayerList
                {
                    Action = 1,
                    UUID = UUID,
                };
                CurrentWorld.Send(packet3);

                if (ResourcePackManager.Animations.TryGetValue(SpawnAnimation, out Animation spawnAnimation))
                {
                    var packet4 = new AnimateEntity
                    {
                        Animation = spawnAnimation.AnimationName,
                        Controller = spawnAnimation.ControllerName,
                        RuntimeId = EntityId
                    };
                    CurrentWorld.Send(packet4);
                }
                else
                {
                    Log.warn($"Unable to find spawn animation by key:{SpawnAnimation}. Make sure animation is registered.");
                }
            });
        }
    }
}

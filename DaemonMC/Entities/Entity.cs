using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;
using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Utils.Text;

namespace DaemonMC.Entities
{
    public abstract class Entity
    {
        public long EntityId { get; set; } = 0;
        public string NameTag { get; set; } = "";
        public World CurrentWorld { get; protected set; }
        public string ActorType { get; protected set; } = "";
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Velocity { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public float YbodyRotation { get; set; } = 0;
        public string SpawnAnimation { get; set; } = "";
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        private long dataValue = 0;

        public virtual void Spawn(World world)
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

            var pk = new AddActor
            {
                EntityId = EntityId,
                ActorType = ActorType,
                Position = Position,
                Metadata = Metadata
            };
            CurrentWorld.Send(pk);

            Log.debug($"Spawned {ActorType} with entityID {EntityId} in {world.LevelName}");
        }

        public virtual void Despawn()
        {
            if (CurrentWorld.Entities.Remove(EntityId))
            {
                var packet = new RemoveActor
                {
                    EntityId = EntityId,
                };
                CurrentWorld.Send(packet);

                Server.AvailableIds.Enqueue(EntityId);
                Log.debug($"Despawned {ActorType} with entityID {EntityId} in {CurrentWorld.LevelName}");
            }
            else
            {
                Log.error($"Couldn't find {ActorType} with entityID {EntityId} in {CurrentWorld.LevelName}");
            }
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;

            ushort header = 0;
            header |= 0x01;
            header |= 0x02;
            header |= 0x04;
            header |= 0x08;
            header |= 0x10;
            header |= 0x20;

            var movePacket = new MoveActorDelta
            {
                EntityId = EntityId,
                Header = header,
                Position = Position
            };
            CurrentWorld.Send(movePacket);
        }

        public void SendMetadata()
        {
            var packet = new SetActorData
            {
                EntityId = EntityId,
                Metadata = Metadata
            };
            CurrentWorld.Send(packet);
        }

        public void SetFlag(ActorFlags flag, bool enable)
        {
            if (enable)
            {
                dataValue |= (1L << (int)flag);
            }
            else
            {
                dataValue &= ~(1L << (int)flag);
            }
            Metadata[ActorData.RESERVED_0] = new Metadata(dataValue);
            SendMetadata();
        }

        public void PlayAnimation(string animationID, long sendTo = -1)
        {
            Animation animation = ResourcePackManager.Animations[animationID];
            var packet = new AnimateEntity
            {
                Animation = animation.AnimationName,
                Controller = animation.ControllerName,
                RuntimeId = EntityId
            };
            if (sendTo == -1)
            {
                CurrentWorld.Send(packet);
            }
            else
            {
                Server.GetPlayer(sendTo).Send(packet);
            }
        }

        public void SetNameTag(string nameTag)
        {
            NameTag = nameTag;
            if (nameTag != "") { Metadata[ActorData.NAME] = new Metadata(nameTag); }
            SendMetadata();
        }
    }
}

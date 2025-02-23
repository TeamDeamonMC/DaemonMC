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
        public World currentWorld { get; protected set; }
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

            foreach (var player in currentWorld.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var pk = new AddActor
                {
                    EntityId = EntityId,
                    ActorType = ActorType,
                    Position = Position,
                    Metadata = Metadata
                };
                pk.EncodePacket(encoder);
            }

            Log.debug($"Spawned {ActorType} with entityID {EntityId} in {world.levelName}");
        }

        public virtual void Despawn()
        {
            if (currentWorld.Entities.Remove(EntityId))
            {
                foreach (var player in currentWorld.onlinePlayers.Values)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(player);
                    var pk = new RemoveActor
                    {
                        EntityId = EntityId,
                    };
                    pk.EncodePacket(encoder);
                }
                Server.availableIds.Enqueue(EntityId);
                Log.debug($"Despawned {ActorType} with entityID {EntityId} in {currentWorld.levelName}");
            }
            else
            {
                Log.error($"Couldn't find {ActorType} with entityID {EntityId} in {currentWorld.levelName}");
            }
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;

            foreach (Player player in currentWorld.onlinePlayers.Values)
            {
                ushort header = 0;
                header |= 0x01;
                header |= 0x02;
                header |= 0x04;
                header |= 0x08;
                header |= 0x10;
                header |= 0x20;

                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var movePk = new MoveActorDelta
                {
                    EntityId = EntityId,
                    Header = header,
                    Position = Position
                };
                movePk.EncodePacket(encoder);
            }
        }

        public void SendMetadata()
        {
            foreach (var player in currentWorld.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var packet = new SetActorData
                {
                    EntityId = EntityId,
                    Metadata = Metadata
                };
                packet.EncodePacket(encoder);
            }
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

        public void PlayAnimation(string animationID)
        {
            Animation animation = ResourcePackManager.Animations[animationID];
            foreach (var player in currentWorld.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var packet = new AnimateEntity
                {
                    mAnimation = animation.AnimationName,
                    mController = animation.ControllerName,
                    mRuntimeId = EntityId
                };
                packet.EncodePacket(encoder);
            }
        }
    }
}

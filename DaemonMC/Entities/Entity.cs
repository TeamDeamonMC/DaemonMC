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
        public World currentLevel { get; protected set; }
        public string ActorType { get; protected set; } = "";
        public Vector3 Position { get; set; } = new Vector3();
        public Vector3 Velocity { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public float YbodyRotation { get; set; } = 0;
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        private long dataValue = 0;

        public void Spawn(World world)
        {
            currentLevel = world;
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

            foreach (var player in currentLevel.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var pk = new AddActor
                {
                    EntityId = EntityId,
                    ActorType = ActorType,
                    Position = Position,
                    Metadata = Metadata
                };
                pk.Encode(encoder);
            }

            Log.debug($"Spawned {ActorType} with entityID {EntityId} in {world.levelName}");
        }

        public void Despawn()
        {
            if (currentLevel.Entities.Remove(EntityId))
            {
                foreach (var player in currentLevel.onlinePlayers.Values)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(player);
                    var pk = new RemoveActor
                    {
                        EntityId = EntityId,
                    };
                    pk.Encode(encoder);
                }
                Server.availableIds.Enqueue(EntityId);
                Log.debug($"Despawned {ActorType} with entityID {EntityId} in {currentLevel.levelName}");
            }
            else
            {
                Log.error($"Couldn't find {ActorType} with entityID {EntityId} in {currentLevel.levelName}");
            }
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;

            foreach (Player player in currentLevel.onlinePlayers.Values)
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
                movePk.Encode(encoder);
            }
        }

        public void SendMetadata()
        {
            Metadata[ActorData.RESERVED_0] = new Metadata(dataValue);

            foreach (var player in currentLevel.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var packet = new SetActorData
                {
                    EntityId = EntityId,
                    Metadata = Metadata
                };
                packet.Encode(encoder);
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
            SendMetadata();
        }
    }
}

using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Entities
{
    public class NPC
    {
        public Vector3 Position { get; set; } = new Vector3(0, 1, 0);
        public Vector2 Rotation { get; set; } = new Vector2(0, 0);
        public World currentWorld { get; set; }
        public Dictionary<ActorData, Metadata> metadata { get; set; } = new Dictionary<ActorData, Metadata>();

        public void Spawn(World world)
        {
            currentWorld = world;
        }
    }
}

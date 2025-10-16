using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Utils
{
    public class PlayerBlockAction
    {
        public int PlayerActionType { get; set; }
        public PlayerActionType ActionType { get; set; }
        public Vector3 Position { get; set; }
        public int Facing { get; set; }
    }
}

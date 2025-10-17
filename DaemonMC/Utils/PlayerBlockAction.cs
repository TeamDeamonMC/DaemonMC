using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Utils
{
    public class PlayerBlockAction
    {
        public int PlayerActionType { get; set; }
        public PlayerActionType ActionType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Facing { get; set; }
    }
}

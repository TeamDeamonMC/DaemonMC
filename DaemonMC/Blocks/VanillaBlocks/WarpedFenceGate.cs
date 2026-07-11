namespace DaemonMC.Blocks
{
    public class WarpedFenceGate : Block
    {
        public WarpedFenceGate()
        {
            Name = "minecraft:warped_fence_gate";

            BlastResistance = 3;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 2;
            Opacity = 0.19999998807907104;

            States["in_wall_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
        }
    }
}

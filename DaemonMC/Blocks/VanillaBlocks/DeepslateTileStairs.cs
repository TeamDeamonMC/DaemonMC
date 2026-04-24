namespace DaemonMC.Blocks
{
    public class DeepslateTileStairs : Block
    {
        public DeepslateTileStairs()
        {
            Name = "minecraft:deepslate_tile_stairs";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3.5;
            Opacity = 0.19999998807907104;

            States["upside_down_bit"] = (byte)0;
            States["weirdo_direction"] = 0;
        }
    }
}

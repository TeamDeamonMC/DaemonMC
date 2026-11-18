namespace DaemonMC.Blocks
{
    public class RedstoneWire : Block
    {
        public RedstoneWire()
        {
            Name = "minecraft:redstone_wire";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0.19999998807907104;

            States["redstone_east"] = "none";
            States["redstone_north"] = "none";
            States["redstone_signal"] = 0;
            States["redstone_south"] = "none";
            States["redstone_west"] = "none";
        }
    }
}

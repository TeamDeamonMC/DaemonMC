namespace DaemonMC.Blocks
{
    public class DarkOakPressurePlate : Block
    {
        public DarkOakPressurePlate()
        {
            Name = "minecraft:dark_oak_pressure_plate";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 0.800000011920929;

            States["redstone_signal"] = 0;
        }
    }
}

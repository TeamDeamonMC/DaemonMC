namespace DaemonMC.Blocks
{
    public class MangrovePressurePlate : Block
    {
        public MangrovePressurePlate()
        {
            Name = "minecraft:mangrove_pressure_plate";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 0.7999999970197678;

            States["redstone_signal"] = 0;
        }
    }
}

namespace DaemonMC.Blocks
{
    public class FlowerPot : Block
    {
        public FlowerPot()
        {
            Name = "minecraft:flower_pot";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["update_bit"] = (byte)0;
        }
    }
}

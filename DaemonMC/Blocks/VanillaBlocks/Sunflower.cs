namespace DaemonMC.Blocks
{
    public class Sunflower : Block
    {
        public Sunflower()
        {
            Name = "minecraft:sunflower";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 60;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0;

            States["upper_block_bit"] = (byte)0;
        }
    }
}

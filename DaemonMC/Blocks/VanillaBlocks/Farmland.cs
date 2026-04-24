namespace DaemonMC.Blocks
{
    public class Farmland : Block
    {
        public Farmland()
        {
            Name = "minecraft:farmland";

            BlastResistance = 0.6000000238418579;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.6000000238418579;
            Opacity = 1;

            States["moisturized_amount"] = 0;
        }
    }
}

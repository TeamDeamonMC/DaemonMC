namespace DaemonMC.Blocks
{
    public class Bamboo : Block
    {
        public Bamboo()
        {
            Name = "minecraft:bamboo";

            BlastResistance = 1;
            Brightness = 0;
            FlameEncouragement = 5;
            Flammability = 20;
            Friction = 0.6000000238418579;
            Hardness = 1;
            Opacity = 0;

            States["age_bit"] = (byte)0;
            States["bamboo_leaf_size"] = "no_leaves";
            States["bamboo_stalk_thickness"] = "thin";
        }
    }
}

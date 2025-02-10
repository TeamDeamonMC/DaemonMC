namespace DaemonMC.Blocks
{
    public class Bamboo : Block
    {
        public Bamboo()
        {
            Name = "minecraft:bamboo";

            States["age_bit"] = (byte)0;
            States["bamboo_leaf_size"] = "no_leaves";
            States["bamboo_stalk_thickness"] = "thin";
        }
    }
}

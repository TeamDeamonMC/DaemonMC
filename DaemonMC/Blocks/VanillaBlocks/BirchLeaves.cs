namespace DaemonMC.Blocks
{
    public class BirchLeaves : Block
    {
        public BirchLeaves()
        {
            Name = "minecraft:birch_leaves";


            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}

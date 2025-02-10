namespace DaemonMC.Blocks
{
    public class CherryLeaves : Block
    {
        public CherryLeaves()
        {
            Name = "minecraft:cherry_leaves";


            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}

namespace DaemonMC.Blocks
{
    public class JungleLeaves : Block
    {
        public JungleLeaves()
        {
            Name = "minecraft:jungle_leaves";


            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}

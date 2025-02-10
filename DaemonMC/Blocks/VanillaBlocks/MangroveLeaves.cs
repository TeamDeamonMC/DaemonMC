namespace DaemonMC.Blocks
{
    public class MangroveLeaves : Block
    {
        public MangroveLeaves()
        {
            Name = "minecraft:mangrove_leaves";


            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}

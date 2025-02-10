namespace DaemonMC.Blocks
{
    public class SculkShrieker : Block
    {
        public SculkShrieker()
        {
            Name = "minecraft:sculk_shrieker";


            States["active"] = (byte)0;
            States["can_summon"] = (byte)0;
        }
    }
}

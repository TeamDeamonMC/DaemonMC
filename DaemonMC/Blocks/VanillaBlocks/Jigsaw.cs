namespace DaemonMC.Blocks
{
    public class Jigsaw : Block
    {
        public Jigsaw()
        {
            Name = "minecraft:jigsaw";


            States["facing_direction"] = 0;
            States["rotation"] = 0;
        }
    }
}

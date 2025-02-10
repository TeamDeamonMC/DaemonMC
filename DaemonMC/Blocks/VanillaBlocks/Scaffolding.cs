namespace DaemonMC.Blocks
{
    public class Scaffolding : Block
    {
        public Scaffolding()
        {
            Name = "minecraft:scaffolding";

            States["stability"] = 0;
            States["stability_check"] = (byte)0;
        }
    }
}

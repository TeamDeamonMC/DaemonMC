namespace DaemonMC.Blocks
{
    public class RedstoneWire : Block
    {
        public RedstoneWire()
        {
            Name = "minecraft:redstone_wire";

            States["redstone_signal"] = 0;
        }
    }
}

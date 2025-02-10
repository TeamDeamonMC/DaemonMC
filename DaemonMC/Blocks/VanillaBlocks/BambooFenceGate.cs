namespace DaemonMC.Blocks
{
    public class BambooFenceGate : Block
    {
        public BambooFenceGate()
        {
            Name = "minecraft:bamboo_fence_gate";

            States["in_wall_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
        }
    }
}

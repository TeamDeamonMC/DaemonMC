namespace DaemonMC.Blocks
{
    public class BirchFenceGate : Block
    {
        public BirchFenceGate()
        {
            Name = "minecraft:birch_fence_gate";

            States["in_wall_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
        }
    }
}

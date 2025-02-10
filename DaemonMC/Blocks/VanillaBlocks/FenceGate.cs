namespace DaemonMC.Blocks
{
    public class FenceGate : Block
    {
        public FenceGate()
        {
            Name = "minecraft:fence_gate";


            States["in_wall_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
        }
    }
}

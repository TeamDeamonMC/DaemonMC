namespace DaemonMC.Blocks
{
    public class PaleOakFenceGate : Block
    {
        public PaleOakFenceGate()
        {
            Name = "minecraft:pale_oak_fence_gate";


            States["in_wall_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
        }
    }
}

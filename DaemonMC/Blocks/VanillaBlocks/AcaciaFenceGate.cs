namespace DaemonMC.Blocks
{
    public class AcaciaFenceGate : Block
    {
        public AcaciaFenceGate()
        {
            Name = "minecraft:acacia_fence_gate";

            States["in_wall_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
        }
    }
}

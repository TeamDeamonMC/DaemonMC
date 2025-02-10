namespace DaemonMC.Blocks
{
    public class CopperDoor : Block
    {
        public CopperDoor()
        {
            Name = "minecraft:copper_door";

            States["door_hinge_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
            States["upper_block_bit"] = (byte)0;
        }
    }
}

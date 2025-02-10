namespace DaemonMC.Blocks
{
    public class OxidizedCopperDoor : Block
    {
        public OxidizedCopperDoor()
        {
            Name = "minecraft:oxidized_copper_door";


            States["door_hinge_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
            States["open_bit"] = (byte)0;
            States["upper_block_bit"] = (byte)0;
        }
    }
}

namespace DaemonMC.Blocks
{
    public class BrewingStand : Block
    {
        public BrewingStand()
        {
            Name = "minecraft:brewing_stand";


            States["brewing_stand_slot_a_bit"] = (byte)0;
            States["brewing_stand_slot_b_bit"] = (byte)0;
            States["brewing_stand_slot_c_bit"] = (byte)0;
        }
    }
}

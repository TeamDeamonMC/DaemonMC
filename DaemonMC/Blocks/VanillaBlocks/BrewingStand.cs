namespace DaemonMC.Blocks
{
    public class BrewingStand : Block
    {
        public BrewingStand()
        {
            Name = "minecraft:brewing_stand";

            BlastResistance = 0.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.5;
            Opacity = 0.19999998807907104;

            States["brewing_stand_slot_a_bit"] = (byte)0;
            States["brewing_stand_slot_b_bit"] = (byte)0;
            States["brewing_stand_slot_c_bit"] = (byte)0;
        }
    }
}

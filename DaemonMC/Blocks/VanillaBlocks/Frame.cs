namespace DaemonMC.Blocks
{
    public class Frame : Block
    {
        public Frame()
        {
            Name = "minecraft:frame";

            BlastResistance = 0.25;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.25;
            Opacity = 0;

            States["facing_direction"] = 0;
            States["item_frame_map_bit"] = (byte)0;
            States["item_frame_photo_bit"] = (byte)0;
        }
    }
}

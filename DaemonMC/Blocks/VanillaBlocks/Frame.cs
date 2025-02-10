namespace DaemonMC.Blocks
{
    public class Frame : Block
    {
        public Frame()
        {
            Name = "minecraft:frame";


            States["facing_direction"] = 0;
            States["item_frame_map_bit"] = (byte)0;
            States["item_frame_photo_bit"] = (byte)0;
        }
    }
}

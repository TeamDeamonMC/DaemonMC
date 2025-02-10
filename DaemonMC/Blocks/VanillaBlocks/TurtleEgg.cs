namespace DaemonMC.Blocks
{
    public class TurtleEgg : Block
    {
        public TurtleEgg()
        {
            Name = "minecraft:turtle_egg";


            States["cracked_state"] = "no_cracks";
            States["turtle_egg_count"] = "one_egg";
        }
    }
}

namespace DaemonMC.Blocks
{
    public class AmethystCluster : Block
    {
        public AmethystCluster()
        {
            Name = "minecraft:amethyst_cluster";

            BlastResistance = 1.5;
            Brightness = 5;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["minecraft:block_face"] = "down";
        }
    }
}

namespace DaemonMC.Blocks
{
    public class PotentSulfur : Block
    {
        public PotentSulfur()
        {
            Name = "minecraft:potent_sulfur";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["potent_sulfur_state"] = "dry";
        }
    }
}

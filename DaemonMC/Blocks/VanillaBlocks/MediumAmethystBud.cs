namespace DaemonMC.Blocks
{
    public class MediumAmethystBud : Block
    {
        public MediumAmethystBud()
        {
            Name = "minecraft:medium_amethyst_bud";

            BlastResistance = 1.5;
            Brightness = 2;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["minecraft:block_face"] = "down";
        }
    }
}

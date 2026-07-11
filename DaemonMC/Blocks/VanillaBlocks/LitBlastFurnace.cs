namespace DaemonMC.Blocks
{
    public class LitBlastFurnace : Block
    {
        public LitBlastFurnace()
        {
            Name = "minecraft:lit_blast_furnace";

            BlastResistance = 3.5;
            Brightness = 13;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3.5;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
        }
    }
}

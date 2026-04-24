namespace DaemonMC.Blocks
{
    public class ChiseledBookshelf : Block
    {
        public ChiseledBookshelf()
        {
            Name = "minecraft:chiseled_bookshelf";

            BlastResistance = 1.5;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.5;
            Opacity = 1;

            States["books_stored"] = 0;
            States["direction"] = 0;
        }
    }
}

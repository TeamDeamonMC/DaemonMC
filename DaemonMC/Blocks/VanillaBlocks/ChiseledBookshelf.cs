namespace DaemonMC.Blocks
{
    public class ChiseledBookshelf : Block
    {
        public ChiseledBookshelf()
        {
            Name = "minecraft:chiseled_bookshelf";

            States["books_stored"] = 0;
            States["direction"] = 0;
        }
    }
}

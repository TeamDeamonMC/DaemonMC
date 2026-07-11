namespace DaemonMC.Blocks
{
    public class BubbleColumn : Block
    {
        public BubbleColumn()
        {
            Name = "minecraft:bubble_column";

            BlastResistance = 0;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0;
            Opacity = 0.10000002384185791;

            States["drag_down"] = (byte)0;
        }
    }
}

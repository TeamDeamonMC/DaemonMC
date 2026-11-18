namespace DaemonMC.Blocks
{
    public class LimeConcreteSlab : Block
    {
        public LimeConcreteSlab()
        {
            Name = "minecraft:lime_concrete_slab";

            BlastResistance = 0.35999998450279236;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 1.7999999523162842;
            Opacity = 1;

            States["minecraft:vertical_half"] = "bottom";
        }
    }
}

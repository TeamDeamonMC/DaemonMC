namespace DaemonMC.Blocks
{
    public class AzaleaLeavesFlowered : Block
    {
        public AzaleaLeavesFlowered()
        {
            Name = "minecraft:azalea_leaves_flowered";

            BlastResistance = 0.20000000298023224;
            Brightness = 0;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0.5;

            States["persistent_bit"] = (byte)0;
            States["update_bit"] = (byte)0;
        }
    }
}

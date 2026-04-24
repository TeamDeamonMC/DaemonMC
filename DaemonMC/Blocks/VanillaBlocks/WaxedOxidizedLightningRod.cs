namespace DaemonMC.Blocks
{
    public class WaxedOxidizedLightningRod : Block
    {
        public WaxedOxidizedLightningRod()
        {
            Name = "minecraft:waxed_oxidized_lightning_rod";

            BlastResistance = 6;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 3;
            Opacity = 0.19999998807907104;

            States["facing_direction"] = 0;
            States["powered_bit"] = (byte)0;
        }
    }
}

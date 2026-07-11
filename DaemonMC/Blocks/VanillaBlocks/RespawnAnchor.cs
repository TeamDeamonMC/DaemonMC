namespace DaemonMC.Blocks
{
    public class RespawnAnchor : Block
    {
        public RespawnAnchor()
        {
            Name = "minecraft:respawn_anchor";

            BlastResistance = 1200;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 50;
            Opacity = 1;

            States["respawn_anchor_charge"] = 0;
        }
    }
}

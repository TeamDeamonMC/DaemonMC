namespace DaemonMC.Blocks
{
    public class GlowLichen : Block
    {
        public GlowLichen()
        {
            Name = "minecraft:glow_lichen";

            BlastResistance = 0.20000000298023224;
            Brightness = 7;
            FlameEncouragement = 15;
            Flammability = 100;
            Friction = 0.6000000238418579;
            Hardness = 0.20000000298023224;
            Opacity = 0;

            States["multi_face_direction_bits"] = 0;
        }
    }
}

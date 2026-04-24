namespace DaemonMC.Blocks
{
    public class EndPortalFrame : Block
    {
        public EndPortalFrame()
        {
            Name = "minecraft:end_portal_frame";

            BlastResistance = 3600000;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = -1;
            Opacity = 0.19999998807907104;

            States["end_portal_eye_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}

namespace DaemonMC.Blocks
{
    public class EndPortalFrame : Block
    {
        public EndPortalFrame()
        {
            Name = "minecraft:end_portal_frame";


            States["end_portal_eye_bit"] = (byte)0;
            States["minecraft:cardinal_direction"] = "south";
        }
    }
}

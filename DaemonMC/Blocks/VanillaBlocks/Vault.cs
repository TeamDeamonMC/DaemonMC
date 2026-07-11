namespace DaemonMC.Blocks
{
    public class Vault : Block
    {
        public Vault()
        {
            Name = "minecraft:vault";

            BlastResistance = 50;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 50;
            Opacity = 1;

            States["minecraft:cardinal_direction"] = "south";
            States["ominous"] = (byte)0;
            States["vault_state"] = "inactive";
        }
    }
}

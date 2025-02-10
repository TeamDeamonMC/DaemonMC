namespace DaemonMC.Blocks
{
    public class Vault : Block
    {
        public Vault()
        {
            Name = "minecraft:vault";


            States["minecraft:cardinal_direction"] = "south";
            States["ominous"] = (byte)0;
            States["vault_state"] = "inactive";
        }
    }
}

namespace DaemonMC.Blocks
{
    public class TrialSpawner : Block
    {
        public TrialSpawner()
        {
            Name = "minecraft:trial_spawner";


            States["ominous"] = (byte)0;
            States["trial_spawner_state"] = 0;
        }
    }
}

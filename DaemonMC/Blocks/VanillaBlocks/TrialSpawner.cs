namespace DaemonMC.Blocks
{
    public class TrialSpawner : Block
    {
        public TrialSpawner()
        {
            Name = "minecraft:trial_spawner";

            BlastResistance = 50;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 50;
            Opacity = 1;

            States["ominous"] = (byte)0;
            States["trial_spawner_state"] = 0;
        }
    }
}

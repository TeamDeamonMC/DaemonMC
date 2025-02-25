namespace DaemonMC.Utils.Game
{
    public class AbilitiesData
    {
        public short Layer { get; set; }
        public int AbilitiesSet { get; set; }
        public int AbilityValues { get; set; }
        public float FlySpeed { get; set; }
        public float VerticalFlySpeed { get; set; }
        public float WalkSpeed { get; set; }

        public AbilitiesData(short layer, int abilitiesSet, int abilityValues, float flySpeed, float verticalFlySpeed, float walkSpeed)
        {
            Layer = layer;
            AbilitiesSet = abilitiesSet;
            AbilityValues = abilityValues;
            FlySpeed = flySpeed;
            VerticalFlySpeed = verticalFlySpeed;
            WalkSpeed = walkSpeed;
        }
    }
}

namespace DaemonMC.Utils.Game
{
    public class AbilitiesData
    {
        public short Layer { get; set; }
        public int AbilitiesSet { get; set; }
        public PermissionSet AbilityValues { get; set; }
        public float FlySpeed { get; set; }
        public float VerticalFlySpeed { get; set; }
        public float WalkSpeed { get; set; }

        public AbilitiesData(short layer, int abilitiesSet, PermissionSet abilityValues, float flySpeed, float verticalFlySpeed, float walkSpeed)
        {
            Layer = layer;
            AbilitiesSet = abilitiesSet;
            AbilityValues = abilityValues;
            FlySpeed = flySpeed;
            VerticalFlySpeed = verticalFlySpeed;
            WalkSpeed = walkSpeed;
        }
    }

    public class PermissionSet
    {
        public bool Build { get; set; }
        public bool Mine { get; set; }
        public bool DoorsAndSwitches { get; set; }
        public bool OpenContainers { get; set; }
        public bool AttackPlayers { get; set; }
        public bool AttackMobs { get; set; }
        public bool OperatorCommands { get; set; }
        public bool Teleport { get; set; }
        public bool MayFly { get; set; }
    }
}

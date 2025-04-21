namespace DaemonMC.Entities
{
    public class EntityLink
    {
        public long EntityId { get; set; } = 0;
        public byte Type { get; set; } = 0; // 0 - dismount, 1 - rider, 2 - passenget
        public float AngularVelocity { get; set; } = 0;
    }
}

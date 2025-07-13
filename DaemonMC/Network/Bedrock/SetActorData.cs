using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class SetActorData : Packet
    {
        public override int Id => (int) Info.Bedrock.SetActorData;

        public long EntityId { get; set; } = 0;
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public SynchedProperties Properties { get; set; } = new SynchedProperties();
        public long Tick { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteMetadata(Metadata);
            encoder.WriteProperties(Properties);
            encoder.WriteVarLong(Tick);
        }
    }
}

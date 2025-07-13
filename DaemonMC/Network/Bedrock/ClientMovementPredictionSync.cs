using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class ClientMovementPredictionSync : Packet
    {
        public override int Id => (int) Info.Bedrock.ClientMovementPredictionSync;

        public List<ActorFlags> ActorData { get; set; } = new List<ActorFlags>();
        public float Scale { get; set; } = 0;
        public float Width { get; set; } = 0;
        public float Heigth { get; set; } = 0;
        public AttributesValues Attributes { get; set; }
        public long EntityId { get; set; } = 0;
        public bool Flying { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {
            ActorData = decoder.Read<ActorFlags>();
            Scale = decoder.ReadFloat();
            Width = decoder.ReadFloat();
            Heigth = decoder.ReadFloat();
            Attributes = decoder.ReadAttributes();
            EntityId = decoder.ReadVarLong();
            Flying = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

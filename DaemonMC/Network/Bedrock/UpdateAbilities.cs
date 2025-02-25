using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class UpdateAbilities : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.UpdateAbilities;

        public long EntityId { get; set; } = 0;
        public byte PlayerPermissions { get; set; } = 0;
        public byte CommandPermissions { get; set; } = 0;
        public List<AbilitiesData> Layers { get; set; } = new List<AbilitiesData>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteLong(EntityId);
            encoder.WriteByte(PlayerPermissions);
            encoder.WriteByte(CommandPermissions);
            encoder.WriteAbilitiesData(Layers);
        }
    }
}

using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class SetHud : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.SetHud;

        public Dictionary<HudElements, bool> HudElements { get; set; } = new Dictionary<HudElements, bool>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(HudElements.Count());
            foreach (var element in HudElements)
            {
                encoder.WriteSignedVarInt((int)element.Key);
                encoder.WriteSignedVarInt(element.Value == true ? 1 : 0);
            }
        }
    }
}

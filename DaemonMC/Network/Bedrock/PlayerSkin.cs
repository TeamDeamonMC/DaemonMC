using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerSkin : Packet
    {
        public override int Id => (int) Info.Bedrock.PlayerSkin;

        public Guid UUID { get; set; } = Guid.NewGuid();
        public Skin Skin { get; set; } = new Skin();
        public string Name { get; set; } = "";
        public string OldName { get; set; } = "";
        public bool Trusted { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {
            UUID = decoder.ReadUUID();
            Skin = decoder.ReadSkin();
            Name = decoder.ReadString();
            OldName = decoder.ReadString();
            Trusted = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteUUID(UUID);
            encoder.WriteSkin(Skin);
            encoder.WriteString(Name);
            encoder.WriteString(OldName);
            encoder.WriteBool(Trusted);
        }
    }
}

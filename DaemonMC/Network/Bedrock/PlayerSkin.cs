using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerSkin : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PlayerSkin;

        public Guid UUID = Guid.NewGuid();
        public Skin playerSkin = new Skin();
        public string Name = "";
        public string oldName = "";
        public bool Trusted = false;

        protected override void Decode(PacketDecoder decoder)
        {
            UUID = decoder.ReadUUID();
            playerSkin = decoder.ReadSkin();
            Name = decoder.ReadString();
            oldName = decoder.ReadString();
            Trusted = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteUUID(UUID);
            encoder.WriteSkin(playerSkin);
            encoder.WriteString(Name);
            encoder.WriteString(oldName);
            encoder.WriteBool(Trusted);
        }
    }
}

using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerSkin
    {
        public Info.Bedrock id = Info.Bedrock.PlayerSkin;

        public Guid UUID = Guid.NewGuid();
        public Skin playerSkin = new Skin();
        public string Name = "";
        public string oldName = "";
        public bool Trusted = false;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new PlayerSkin
            {
                UUID = decoder.ReadUUID(),
                playerSkin = decoder.ReadSkin(),
                Name = decoder.ReadString(),
                oldName = decoder.ReadString(),
                Trusted = decoder.ReadBool()
            };

            decoder.player.PacketEvent_PlayerSkin(packet);
        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteUUID(UUID);
            encoder.WriteSkin(playerSkin);
            encoder.WriteString(Name);
            encoder.WriteString(oldName);
            encoder.WriteBool(Trusted);
            encoder.handlePacket();
        }
    }
}

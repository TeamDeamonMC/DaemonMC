using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerSkin
    {
        public Info.Bedrock id = Info.Bedrock.PlayerSkin;

        public Guid UUID = Guid.NewGuid();
        public Skin playerSkin = new Skin();

        public void Decode(PacketDecoder decoder)
        {
            var packet = new PlayerSkin
            {
                UUID = decoder.ReadUUID(),
            };
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}

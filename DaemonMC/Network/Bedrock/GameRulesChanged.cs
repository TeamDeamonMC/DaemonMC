using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class GameRulesChanged
    {
        public Info.Bedrock id = Info.Bedrock.GameRulesChanged;

        public Dictionary<string, GameRule> GameRules = new Dictionary<string, GameRule>();

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteGameRulesData(GameRules);
            encoder.handlePacket();
        }
    }
}

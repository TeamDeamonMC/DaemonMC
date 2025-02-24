using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class GameRulesChanged : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.GameRulesChanged;

        public Dictionary<string, GameRule> GameRules { get; set; } = new Dictionary<string, GameRule>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteGameRulesData(GameRules);
        }
    }
}

using DaemonMC.Network.Bedrock;

namespace DaemonMC.Plugin.Events;

public class PlayerSentMessageEvent(Player player, TextMessage textMessage) : Event {
    
    private Player Player { get; } = player;
    private string UncensoredMessage { get; set; } = textMessage.Message;
    private string CensoredMessage { get; set; } = textMessage.FilteredMessage;
    
    public Player GetPlayer() {
        return Player;
    }
    
    public string GetUncensoredMessage() {
        return UncensoredMessage;
    }
    
    public string GetCensoredMessage() {
        return CensoredMessage;
    }
}
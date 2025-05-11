using DaemonMC.Utils;

namespace DaemonMC.Plugin.Events;

public class PlayerLeavedEvent(Player player) : Event {
    
    private Player Player { get; } = player;
    private string LeaveMessage { get; set; } = $"{TextFormat.Yellow}{player.Username} left the server!";
    
    public Player GetPlayer() {
        return Player;
    }
    
    public string GetLeaveMessage() {
        return LeaveMessage;
    }
    
    public void SetLeaveMessage(string message) {
        LeaveMessage = message;
    }
}
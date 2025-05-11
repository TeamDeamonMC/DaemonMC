using DaemonMC.Utils;

namespace DaemonMC.Plugin.Events;

public class PlayerJoinedEvent(Player player) : Event {
    
    private Player Player { get; } = player;
    private string JoinMessage { get; set; } = $"{TextFormat.Yellow}{player.Username} joined the server!";
    private bool _DisableJoinMessage { get; set; }
    
    public Player GetPlayer() {
        return Player;
    }
    
    public string GetJoinMessage() {
        return JoinMessage;
    }
    
    public void SetJoinMessage(string message) {
        JoinMessage = message;
    }

    public bool IsJoinMessageEnabled() {
        return !_DisableJoinMessage;
    }

    public void DisableJoinMessage() {
        _DisableJoinMessage = true;
    }
}
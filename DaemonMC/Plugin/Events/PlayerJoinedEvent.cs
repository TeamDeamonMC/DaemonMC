namespace DaemonMC.Plugin.Events;

public class PlayerJoinedEvent(Player player) : Event {

    private Player Player { get; } = player;
    
    public Player GetPlayer() {
        return Player;
    }
}

namespace DaemonMC.Plugin.Events;

public class PlayerMoveEvent(Player player) : Event {

    private Player Player { get; } = player;
    
    public Player GetPlayer() {
        return Player;
    }
}

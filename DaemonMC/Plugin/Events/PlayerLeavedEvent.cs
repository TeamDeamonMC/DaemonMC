namespace DaemonMC.Plugin.Events;

public class PlayerLeavedEvent(Player player) : Event {

    private Player Player { get; } = player;

    public Player GetPlayer() {
        return Player;
    }
}

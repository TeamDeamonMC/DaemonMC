namespace DaemonMC.Plugin.Events;

public class PlayerAttackedPlayerEvent(Player attacker, Player victim) : Event {
    
    private Player Attacker { get; } = attacker;
    private Player Victim { get; } = victim;
    
    public Player GetAttacker() {
        return Attacker;
    }

    public Player GetVictim() {
        return Victim;
    }
}
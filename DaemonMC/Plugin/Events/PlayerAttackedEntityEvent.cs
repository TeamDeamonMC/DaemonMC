using DaemonMC.Entities;

namespace DaemonMC.Plugin.Events;

public class PlayerAttackedEntityEvent(Player player, Entity entity) : Event {

    private Player Player { get; } = player;
    private Entity Entity { get; } = entity;
    
    public Player GetPlayer() {
        return Player;
    }

    public Entity GetEntity() {
        return Entity;
    }
}

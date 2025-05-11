using DaemonMC.Network.Bedrock;
using DaemonMC.Utils;

namespace DaemonMC.Plugin.Events;

public class PlayerSkinChangedEvent(Player player, PlayerSkin playerSkin) : Event {
    
    private Player Player { get; } = player;
    
    public Player GetPlayer() {
        return Player;
    }
    
    public Skin GetSkin() {
        return playerSkin.Skin;
    }

    public string GetSkinName() {
        return playerSkin.Name;
    }

    public string GetOldSkinName() {
        return playerSkin.OldName;
    }

    public bool IsSkinTrusted() {
        return playerSkin.Trusted;
    }
}
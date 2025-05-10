using System.Net;
using DaemonMC.Network;
using DaemonMC.Plugin.Events;

namespace DaemonMC.Plugin;

public interface IPlugin {
    void OnLoad();
    void OnUnload();
    
    void OnPlayerJoined(PlayerJoinedEvent ev);
    void OnPlayerLeaved(PlayerLeavedEvent ev);
    void OnPlayerMove(PlayerMoveEvent ev);
    void OnPlayerAttackedEntity(PlayerAttackedEntityEvent ev);
    void OnPlayerAttackedPlayer(PlayerAttackedPlayerEvent ev);
    void OnPlayerSentMessage(PlayerSentMessageEvent ev);
    void OnPlayerSkinChanged(PlayerSkinChangedEvent ev);
    bool OnPacketReceived(IPEndPoint ep, Packet packet);
    bool OnPacketSent(IPEndPoint ep, Packet packet);
}

public abstract class Plugin : IPlugin {
    
    private string Name { get; set; } = "Plugin";
    private string Version { get; set; } = "1.0.0";
    internal PluginResources Resources { get; set; } = null!;

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }
    public virtual void OnPlayerJoined(PlayerJoinedEvent ev) { }
    public virtual void OnPlayerLeaved(PlayerLeavedEvent ev) { }
    public virtual void OnPlayerMove(PlayerMoveEvent ev) { }
    public virtual void OnPlayerAttackedEntity(PlayerAttackedEntityEvent ev) { }
    public virtual void OnPlayerAttackedPlayer(PlayerAttackedPlayerEvent ev) { }
    public virtual void OnPlayerSentMessage(PlayerSentMessageEvent ev) { }
    public virtual void OnPlayerSkinChanged(PlayerSkinChangedEvent ev) { }
    public virtual bool OnPacketReceived(IPEndPoint ep, Packet packet) { return true; }
    public virtual bool OnPacketSent(IPEndPoint ep, Packet packet) { return true; }

    public PluginResources GetPluginResources() {
        return Resources;
    }

    public void SetName(string name) {
        Name = name;
    }
    
    public void SetVersion(string version) {
        Version = version;
    }
    
    public string GetName() {
        return Name;
    }
    
    public string GetVersion() {
        return Version;
    }
}

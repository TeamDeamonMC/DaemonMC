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
    void OnPacketReceived(PacketEvent ev);
    void OnPacketSent(PacketEvent ev);
}

public interface HotReload {
    void OnReload();
}

public abstract class Plugin : IPlugin
{

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }
    public virtual void OnReload() { }
    public virtual void OnPlayerJoined(PlayerJoinedEvent ev) { }
    public virtual void OnPlayerLeaved(PlayerLeavedEvent ev) { }
    public virtual void OnPlayerMove(PlayerMoveEvent ev) { }
    public virtual void OnPlayerAttackedEntity(PlayerAttackedEntityEvent ev) { }
    public virtual void OnPlayerAttackedPlayer(PlayerAttackedPlayerEvent ev) { }
    public virtual void OnPlayerSentMessage(PlayerSentMessageEvent ev) { }
    public virtual void OnPlayerSkinChanged(PlayerSkinChangedEvent ev) { }
    public virtual void OnPacketReceived(PacketEvent ev) { }
    public virtual void OnPacketSent(PacketEvent ev) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class PluginInfo : Attribute
{
    public string Name { get; }
    public string Version { get; }
    public string[] Dependencies { get; }

    public PluginInfo(string name, string version, params string[] dependencies)
    {
        Name = name;
        Version = version;
        Dependencies = dependencies;
    }
}

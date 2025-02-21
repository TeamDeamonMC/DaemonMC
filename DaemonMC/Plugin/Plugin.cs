using System.Net;
using DaemonMC.Network;

namespace DaemonMC.Plugin.Plugin
{
    public interface IPlugin
    {
        void OnLoad();
        void OnUnload();

        void OnPlayerJoined(Player player);
        bool OnPacketReceived(IPEndPoint ep, Packet packet);
        bool OnPacketSent(IPEndPoint ep, Packet packet);
    }

    public abstract class Plugin : IPlugin
    {
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
        public virtual void OnPlayerJoined(Player player) { }
        public virtual bool OnPacketReceived(IPEndPoint ep, Packet packet) { return true; }
        public virtual bool OnPacketSent(IPEndPoint ep, Packet packet) { return true; }
    }
}

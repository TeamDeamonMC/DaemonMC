using System.Net;
using DaemonMC.Entities;
using DaemonMC.Network;

namespace DaemonMC.Plugin.Plugin
{
    public interface IPlugin
    {
        void OnLoad();
        void OnUnload();

        void OnPlayerJoined(Player player);
        void OnPlayerMove(Player player);
        bool OnPacketReceived(IPEndPoint ep, Packet packet);
        bool OnPacketSent(IPEndPoint ep, Packet packet);
        void OnEntityAttack(Player player, Entity entity);
        void OnPlayerAttack(Player player, Player victim);
    }

    public abstract class Plugin : IPlugin
    {
        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
        public virtual void OnPlayerJoined(Player player) { }
        public virtual void OnPlayerMove(Player player) { }
        public virtual bool OnPacketReceived(IPEndPoint ep, Packet packet) { return true; }
        public virtual bool OnPacketSent(IPEndPoint ep, Packet packet) { return true; }
        public virtual void OnEntityAttack(Player player, Entity entity) { }
        public virtual void OnPlayerAttack(Player player, Player victim) { }
    }
}

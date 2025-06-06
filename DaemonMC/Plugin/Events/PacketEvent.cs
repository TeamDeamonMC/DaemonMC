using System.Net;
using DaemonMC.Network;
using DaemonMC.Network.RakNet;

namespace DaemonMC.Plugin.Events;

public class PacketEvent(IPEndPoint ep, Packet packet) : Event {

    private IPEndPoint EndPoint { get; } = ep;
    private Packet Packet { get; } = packet;

    public Player GetPlayer()
    {
        return Server.GetPlayer(RakSessionManager.getSession(ep).EntityID);
    }

    public IPEndPoint GetEndPoint() {
        return EndPoint;
    }

    public Packet GetPacket() {
        return Packet;
    }
}

using System.Net;
using BasaltBE.NetherNet.Types;

namespace DaemonMC.Network.NetherNet
{
    public sealed class NetherNetEndPoint : IPEndPoint
    {
        public NethernetPeer Peer { get; }

        public NetherNetEndPoint(NethernetPeer peer) : base(peer.Address ?? IPAddress.None, 0)
        {
            Peer = peer;
        }

        public override bool Equals(object? comparand)
        {
            return comparand is NetherNetEndPoint other && other.Peer.Equals(Peer);
        }

        public override int GetHashCode()
        {
            return Peer.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Address} (NetherNet {Peer.NetworkId})";
        }
    }
}

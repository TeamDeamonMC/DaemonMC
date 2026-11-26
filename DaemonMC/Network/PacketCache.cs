using System.Collections.Concurrent;

namespace DaemonMC.Network
{
    public static class PacketCache
    {
        private static volatile ConcurrentDictionary<(string Key, int Protocol), byte[]> entries = new();

        public static void Send(Player player, Packet packet, string? key = null)
        {
            var encoder = PacketEncoderPool.Get(player);
            var current = entries;
            byte[] bytes = current.GetOrAdd((key ?? packet.GetType().FullName!, encoder.protocolVersion), k => packet.Serialize(k.Protocol));
            packet.EncodeSerialized(encoder, bytes);
        }

        public static void Invalidate()
        {
            entries = new();
        }
    }
}

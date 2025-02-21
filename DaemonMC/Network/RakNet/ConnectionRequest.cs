using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class ConnectionRequestPacket
    {
        public long Time { get; set; }
        public long GUID { get; set; }
        public byte Security { get; set; }
    }

    public class ConnectionRequest
    {
        public static byte id = 9;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new ConnectionRequestPacket
            {
                GUID = decoder.ReadLong(),
                Time = decoder.ReadLongLE(),
                Security = decoder.ReadByte() //todo
            };

            RakPacketProcessor.ConnectionRequest(packet, decoder.clientEp);
        }

        public static void Encode(ConnectionRequestPacket fields, PacketEncoder encoder)
        {
            encoder.WriteByte(id);
            encoder.WriteLong(fields.GUID);
            encoder.WriteLongLE(fields.Time);
            encoder.WriteByte(fields.Security);
            encoder.handlePacket("raknet");
        }
    }
}

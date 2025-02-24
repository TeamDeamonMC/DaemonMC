using System.Net;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.RakNet
{
    public class RakPacketProcessor
    {
        public static int mtuMax = 1500;
        public static void UnconnectedPing(UnconnectedPingPacket packet, IPEndPoint clientEp)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new UnconnectedPongPacket
            {
                Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                GUID = 1234567890123456789,
                Magic = "00ffff00fefefefefdfdfdfd12345678",
                MOTD = $"MCPE;{DaemonMC.Servername};100;{Info.Version};0;{DaemonMC.MaxOnline};12345678912345678912;{DaemonMC.Worldname};Survival;1;19132;19133;"
            };
            UnconnectedPong.Encode(pk, encoder);
        }

        public static void OpenConnectionRequest1(OpenConnectionRequest1Packet packet, IPEndPoint clientEp)
        {
            Log.debug($"{clientEp.Address} requested MTU size:{packet.Mtu}. Replying with MTU size:{packet.Mtu} ...");
            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new OpenConnectionReply1Packet
            {
                Magic = "00ffff00fefefefefdfdfdfd12345678",
                GUID = 1234567890123456789,
                Mtu = packet.Mtu
            };
            OpenConnectionReply1.Encode(pk, encoder);
        }

        public static void OpenConnectionRequest2(OpenConnectionRequest2Packet packet, IPEndPoint clientEp)
        {
            Log.debug($"{clientEp.Address} accepted connection with MTU size:{packet.Mtu}");
            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new OpenConnectionReply2Packet
            {
                Magic = "00ffff00fefefefefdfdfdfd12345678",
                GUID = 1234567890123456789,
                Mtu = packet.Mtu
            };
            RakSessionManager.getSession(clientEp).MTU = packet.Mtu;
            OpenConnectionReply2.Encode(pk, encoder);
        }

        public static void ConnectionRequest(ConnectionRequestPacket packet, IPEndPoint clientEp)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new ConnectionRequestAcceptedPacket
            {
                Time = packet.Time,
            };
            RakSessionManager.addSession(clientEp, packet.GUID);
            //Console.WriteLine($"[Connection Request] --clientId: {packet.GUID}  time: {packet.Time} security: {packet.Security}");
            ConnectionRequestAccepted.Encode(pk, encoder);
        }

        public static void ACK(ACKPacket packet, IPEndPoint clientEp)
        {
            var sentPackets = RakSessionManager.getSession(clientEp).sentPackets;

            foreach (var ack in packet.ACKs)
            {
                if (ack.singleSequence)
                {
                    if (!sentPackets.Remove(ack.sequenceNumber, out var data))
                    {
                        Log.debug($"[RakNet] Unable to ACK {ack.sequenceNumber} for {clientEp.Address}. Unexpected sequence number.", ConsoleColor.DarkYellow);
                    }
                }
                else
                {
                    for (uint seq = ack.firstSequenceNumber; seq <= ack.lastSequenceNumber; seq++)
                    {
                        if (!sentPackets.Remove(seq, out var data))
                        {
                            Log.debug($"[RakNet] Unable to ACK {seq} for {clientEp.Address}. Unexpected sequence number.", ConsoleColor.DarkYellow);
                        }
                    }
                }
            }
        }

        public static void NACK(NACKPacket packet, IPEndPoint clientEp)
        {
            foreach (var nack in packet.NACKs)
            {
                if (nack.singleSequence)
                {
                    Reliability.ResendPacket(nack.sequenceNumber, clientEp);
                }
                else
                {
                    for (uint seq = nack.firstSequenceNumber; seq <= nack.lastSequenceNumber; seq++)
                    {
                        Reliability.ResendPacket(seq, clientEp);
                    }
                }
            }
        }

        public static void NewIncomingConnection(NewIncomingConnectionPacket packet)
        {
            //Log.warn($"NewIncomingConnectionPacket: {packet.serverAddress.IPAddress[0]}.{packet.serverAddress.IPAddress[1]}.{packet.serverAddress.IPAddress[2]}.{packet.serverAddress.IPAddress[3]}:{packet.serverAddress.Port} / {packet.incommingTime} / {packet.serverTime} / {packet.internalAddress.Count()}");
        }

        public static void ConnectedPing(ConnectedPingPacket packet, IPEndPoint clientEp)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new ConnectedPongPacket
            {
                pingTime = packet.Time,
                pongTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            };
            ConnectedPong.Encode(pk, encoder);
        }

        public static void Disconnect(RakDisconnectPacket packet, IPEndPoint clientEp)
        {
            if (RakSessionManager.getSession(clientEp) != null)
            {
                Server.RemovePlayer(RakSessionManager.getSession(clientEp).EntityID);
            }
            RakSessionManager.deleteSession(clientEp);
        }
    }
}

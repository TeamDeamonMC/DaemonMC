using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.RakNet
{
    public class RakPacketProcessor
    {
        internal static void HandlePacket(Packet packet, IPEndPoint clientEp)
        {
            if (packet is UnconnectedPing unconnectedPing)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new UnconnectedPong
                {
                    Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    GUID = 1234567890123456789,
                    Magic = [0x00, 0xFF, 0xFF, 0x00, 0xFE, 0xFE, 0xFE, 0xFE, 0xFD, 0xFD, 0xFD, 0xFD, 0x12, 0x34, 0x56, 0x78],
                    MOTD = $"MCPE;{DaemonMC.Servername};100;{Info.Version};0;{DaemonMC.MaxOnline};12345678912345678912;{DaemonMC.Worldname};Survival;1;19132;19133;"
                };
                pk.EncodePacket(encoder);
            }

            if (packet is OpenConnectionRequest1 openConnectionRequest1)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new OpenConnectionReply1
                {
                    Magic = [0x00, 0xFF, 0xFF, 0x00, 0xFE, 0xFE, 0xFE, 0xFE, 0xFD, 0xFD, 0xFD, 0xFD, 0x12, 0x34, 0x56, 0x78],
                    GUID = 1234567890123456789,
                    Mtu = openConnectionRequest1.Mtu
                };
                pk.EncodePacket(encoder);
                Log.debug($"{clientEp} requested MTU size:{openConnectionRequest1.Mtu}. Replying with MTU size:{openConnectionRequest1.Mtu} ...");
            }

            if (packet is OpenConnectionRequest2 openConnectionRequest2)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new OpenConnectionReply2
                {
                    Magic = [0x00, 0xFF, 0xFF, 0x00, 0xFE, 0xFE, 0xFE, 0xFE, 0xFD, 0xFD, 0xFD, 0xFD, 0x12, 0x34, 0x56, 0x78],
                    GUID = 1234567890123456789,
                    Mtu = openConnectionRequest2.Mtu
                };
                RakSessionManager.getSession(clientEp).MTU = openConnectionRequest2.Mtu;
                pk.EncodePacket(encoder);
                Log.debug($"... {clientEp} accepted connection with MTU size:{openConnectionRequest2.Mtu}");
            }

            if (packet is ConnectionRequest connectionRequest)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new ConnectionRequestAccepted
                {
                    Time = connectionRequest.Time,
                };
                RakSessionManager.addSession(clientEp, connectionRequest.GUID);
                pk.EncodePacket(encoder);
            }

            if (packet is ACK ack)
            {
                var sentPackets = RakSessionManager.getSession(clientEp).sentPackets;

                foreach (var acks in ack.ACKs)
                {
                    if (acks.singleSequence)
                    {
                        if (!sentPackets.Remove(acks.sequenceNumber, out var data))
                        {
                            Log.debug($"[RakNet] Unable to ACK {acks.sequenceNumber} for {clientEp.Address}. Unexpected sequence number.", ConsoleColor.DarkYellow);
                        }
                    }
                    else
                    {
                        for (uint seq = acks.firstSequenceNumber; seq <= acks.lastSequenceNumber; seq++)
                        {
                            if (!sentPackets.Remove(seq, out var data))
                            {
                                Log.debug($"[RakNet] Unable to ACK {seq} for {clientEp.Address}. Unexpected sequence number.", ConsoleColor.DarkYellow);
                            }
                        }
                    }
                }
            }

            if (packet is NACK nack)
            {
                foreach (var nacks in nack.NACKs)
                {
                    if (nacks.singleSequence)
                    {
                        Reliability.ResendPacket(nacks.sequenceNumber, clientEp);
                    }
                    else
                    {
                        for (uint seq = nacks.firstSequenceNumber; seq <= nacks.lastSequenceNumber; seq++)
                        {
                            Reliability.ResendPacket(seq, clientEp);
                        }
                    }
                }
            }

            if (packet is NewIncomingConnection newIncomingConnection)
            {
                DateTimeOffset incomingTime = DateTimeOffset.FromUnixTimeSeconds(newIncomingConnection.incommingTime);
                DateTimeOffset serverTime = DateTimeOffset.FromUnixTimeSeconds(newIncomingConnection.serverTime);

                Log.debug($"[RakNet] New Connection: {clientEp.Address} incommingTime: {incomingTime.ToString("yyyy.MM.dd HH:mm:ss")} serverTime: {serverTime.ToString("yyyy.MM.dd HH:mm:ss")}", ConsoleColor.DarkYellow);
            }

            if (packet is ConnectedPing connectedPing)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new ConnectedPong
                {
                    pingTime = connectedPing.Time,
                    pongTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                };
                pk.EncodePacket(encoder);
            }

            if (packet is GamePacket gamePacket)
            {
                BedrockPacketDecoder.BedrockDecoder(PacketDecoderPool.Get(gamePacket.Payload, clientEp));
            }

            if (packet is RakDisconnect disconnect)
            {
                if (RakSessionManager.getSession(clientEp) != null)
                {
                    Server.RemovePlayer(RakSessionManager.getSession(clientEp).EntityID);
                }
                RakSessionManager.deleteSession(clientEp);
            }
        }
    }
}

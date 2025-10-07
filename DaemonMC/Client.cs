using System.Net;
using System.Net.Sockets;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using Newtonsoft.Json;

namespace DaemonMC
{
    public class Client
    {
        public int ConnectionState = 0;
        public bool Running = true;
        public Socket Sock { get; set; } = null!;
        public string Username { get; set; } = "Daemon123";
        public int ProtocolVersion { get; set; } = 0;
        public IPEndPoint iep = null!;
        public byte[] bufferAlloc = new byte[8192];

        public event Func<Packet, bool>? OnPacketReceived;

        public Client()
        {
        }

        public void Run()
        {
            Sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            Sock.Bind(new IPEndPoint(IPAddress.Any, 50001));
            iep = new IPEndPoint(IPAddress.Broadcast, 19132);
            RakSessionManager.createSession(iep);
            var session = RakSessionManager.getSession(iep);
            session.username = Username;
            session.isClient = true;
            session.client = this;
            session.protocolVersion = ProtocolVersion;

            OnPacketReceived += packet =>
            {
                if (packet is UnconnectedPong pong)
                {
                    Console.WriteLine($"MOTD: {pong.MOTD}");
                }
                if (packet is PlayStatus playStatus)
                {
                    Console.WriteLine($"PlayStatus: {(PlayStatusTypes)playStatus.Status}");
                }
                if (packet is Disconnect disconnect)
                {
                    Console.WriteLine($"Disconnect: {disconnect.Reason}");
                    ConnectionState = 0;
                }
                if (packet is NetworkSettings networkSettings)
                {
                    Console.WriteLine($"NetworkSettings: {JsonConvert.SerializeObject(networkSettings, Formatting.Indented)}");
                    session.initCompression = true;


                    PacketEncoder encoder = PacketEncoderPool.Get(iep);
                    var pk = new Login
                    {
                        ProtocolVersion = ProtocolVersion,
                        Request = LoginHandler.createRequest(),
                    };
                    pk.EncodePacket(encoder);
                }
                return false;
            };

            var receiveThread = new Thread(() => {

                while (Running)
                {
                    try
                    {
                        Span<byte> spanBuffer = bufferAlloc.AsSpan();

                        int recv = Sock.Receive(bufferAlloc);

                        if (recv != 0)
                        {
                            var receivedData = spanBuffer.Slice(0, recv);

                            PacketDecoder decoder = PacketDecoderPool.Get(receivedData.ToArray(), iep);
                            decoder.RakDecoder(decoder, recv);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
            receiveThread.Start();

            var pingThread = new Thread(() =>
            {
                while (true)
                {
                    if (ConnectionState == 0)
                    {
                        PacketEncoder encoder = PacketEncoderPool.Get(iep);
                        var pk = new UnconnectedPing
                        {
                            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                            Magic = "00ffff00fefefefefdfdfdfd12345678",
                            ClientId = 123456,
                        };
                        pk.EncodePacket(encoder);
                        encoder.Reset();
                    }
                    else if (ConnectionState == 1)
                    {
                        PacketEncoder encoder = PacketEncoderPool.Get(iep);
                        var pk = new ConnectedPing
                        {
                            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        };
                        pk.EncodePacket(encoder);
                        encoder.Reset();
                    }
                    Thread.Sleep(ConnectionState == 0 ? 1000 : 5000);
                }

            });
            pingThread.Start();
        }

        public bool PacketReceivedEvent(Packet packet)
        {
            if (OnPacketReceived == null)
                return false;

            bool handled = false;
            foreach (Func<Packet, bool> handler in OnPacketReceived.GetInvocationList())
            {
                try
                {
                    if (handler(packet))
                        handled = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Packet event handler error: {ex}");
                }
            }

            return handled;
        }
    }
}

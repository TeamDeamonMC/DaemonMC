using System.Net;
using System.Net.Sockets;
using DaemonMC;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;

namespace DeamonClient;

internal static class Program
{
    private const string DefaultHost = "127.0.0.1";
    private const int DefaultPort = 19132;
    private static readonly int DefaultProtocol = Info.v1_26_20;
    private const string Magic = "00ffff00fefefefefdfdfdfd12345678";

    private static volatile bool _running = true;

    private static void Main(string[] args)
    {
        string host = args.Length > 0 ? args[0] : DefaultHost;
        int port = args.Length > 1 && int.TryParse(args[1], out var parsedPort) ? parsedPort : DefaultPort;
        int protocolVersion = args.Length > 2 && int.TryParse(args[2], out var parsedProtocol)
            ? parsedProtocol
            : DefaultProtocol;

        var remoteAddress = ResolveAddress(host);
        var remoteEp = new IPEndPoint(remoteAddress, port);
        var localEp = new IPEndPoint(IPAddress.Any, 0);

        Console.WriteLine($"Connecting to {remoteEp} with protocol {protocolVersion}.");

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.Bind(localEp);
        socket.Connect(remoteEp);

        var client = new Client
        {
            Sock = socket,
            iep = remoteEp,
            Username = "DeamonClient",
            ProtocolVersion = protocolVersion,
            Running = true,
            ConnectionState = 0
        };

        RakSessionManager.createSession(remoteEp);
        var session = RakSessionManager.getSession(remoteEp);
        session.username = client.Username;
        session.isClient = true;
        session.client = client;
        session.protocolVersion = protocolVersion;

        client.OnPacketReceived += packet =>
        {
            switch (packet)
            {
                case UnconnectedPong pong:
                    Console.WriteLine($"MOTD: {pong.MOTD}");
                    break;
                case ConnectedPong pong:
                    Console.WriteLine($"Connected pong: ping={pong.pingTime}, pong={pong.pongTime}");
                    break;
                case Disconnect disconnect:
                    Console.WriteLine($"Disconnected: {disconnect.Message}");
                    client.ConnectionState = 0;
                    _running = false;
                    break;
                case NetworkSettings networkSettings:
                    Console.WriteLine("Network settings received, enabling compression.");
                    session.initCompression = true;
                    break;
                case ResourcePacksInfo:
                case ResourcePackStack:
                    Console.WriteLine($"Received {packet.GetType().Name}");
                    break;
            }

            return false;
        };

        var receiveThread = new Thread(() => ReceiveLoop(client, remoteEp))
        {
            IsBackground = true
        };
        receiveThread.Start();

        var sendThread = new Thread(() => PingLoop(client, remoteEp))
        {
            IsBackground = true
        };
        sendThread.Start();

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            _running = false;
        };

        while (_running)
        {
            Thread.Sleep(200);
        }

        client.Running = false;
        socket.Close();
    }

    private static void ReceiveLoop(Client client, IPEndPoint remoteEp)
    {
        byte[] buffer = new byte[8192];

        while (_running && client.Running)
        {
            try
            {
                int received = client.Sock.Receive(buffer);
                if (received <= 0)
                {
                    continue;
                }

                var payload = new byte[received];
                Array.Copy(buffer, payload, received);

                var decoder = PacketDecoderPool.Get(payload, remoteEp);
                decoder.RakDecoder(decoder, received);
            }
            catch (SocketException)
            {
                if (_running)
                {
                    Console.WriteLine("Socket closed or unreachable.");
                }
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private static void PingLoop(Client client, IPEndPoint remoteEp)
    {
        while (_running && client.Running)
        {
            try
            {
                if (client.ConnectionState == 0)
                {
                    SendUnconnectedPing(remoteEp);
                    Thread.Sleep(1000);
                }
                else if (client.ConnectionState == 1)
                {
                    SendConnectedPing(remoteEp);
                    Thread.Sleep(5000);
                }
                else
                {
                    Thread.Sleep(250);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Thread.Sleep(1000);
            }
        }
    }

    private static void SendUnconnectedPing(IPEndPoint remoteEp)
    {
        var encoder = PacketEncoderPool.Get(remoteEp);
        var packet = new UnconnectedPing
        {
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Magic = Magic,
            ClientId = Random.Shared.NextInt64()
        };
        packet.EncodePacket(encoder);
    }

    private static void SendConnectedPing(IPEndPoint remoteEp)
    {
        var encoder = PacketEncoderPool.Get(remoteEp);
        var packet = new ConnectedPing
        {
            Time = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        packet.EncodePacket(encoder);
    }

    private static IPAddress ResolveAddress(string host)
    {
        if (IPAddress.TryParse(host, out var address))
        {
            return address;
        }

        var addresses = Dns.GetHostAddresses(host);
        var ipv4 = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
        if (ipv4 != null)
        {
            return ipv4;
        }

        throw new InvalidOperationException($"Unable to resolve host '{host}'.");
    }
}

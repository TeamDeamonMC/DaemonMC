using System.Net;
using System.Net.Sockets;
using DaemonMC;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;

namespace DeamonClient;

internal static class Program
{
    private const string DefaultHost = "127.0.0.1";
    private const int DefaultPort = 19132;
    private static readonly int DefaultProtocol = Info.v1_26_20;
    private const string Magic = "00ffff00fefefefefdfdfdfd12345678";
    private static readonly long ClientGuid = Random.Shared.NextInt64();
    private const string AuthTokenEnv = "";
    
    private static volatile bool _running = true;
    private static volatile bool _loginRequested;

    private static void Main(string[] args)
    {
        string host = args.Length > 0 ? args[0] : DefaultHost;
        int port = args.Length > 1 && int.TryParse(args[1], out var parsedPort) ? parsedPort : DefaultPort;
        int protocolVersion = args.Length > 2 && int.TryParse(args[2], out var parsedProtocol)
            ? parsedProtocol
            : DefaultProtocol;
        string? authToken = AuthTokenEnv;

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
        session.GUID = ClientGuid;

        client.OnPacketReceived += packet =>
        {
            switch (packet)
            {
                case UnconnectedPong pong:
                    Console.WriteLine($"MOTD: {pong.MOTD}");
                    break;
                case OpenConnectionReply1 reply1:
                    Console.WriteLine($"OpenConnectionReply1: mtu={reply1.Mtu}, guid={reply1.GUID}");
                    session.MTU = reply1.Mtu;
                    break;
                case OpenConnectionReply2 reply2:
                    Console.WriteLine($"OpenConnectionReply2: mtu={reply2.Mtu}, guid={reply2.GUID}");
                    session.MTU = reply2.Mtu;
                    break;
                case ConnectionRequestAccepted accepted:
                    Console.WriteLine($"ConnectionRequestAccepted: client={accepted.ClientAddress.Port}, systemIndex={accepted.SystemIndex}");
                    Console.WriteLine("RakNet handshake continuing with NewIncomingConnection and RequestNetworkSettings.");
                    break;
                case NewIncomingConnection newIncomingConnection:
                    Console.WriteLine($"NewIncomingConnection: server={newIncomingConnection.serverAddress.Port}");
                    client.ConnectionState = 1;
                    break;
                case RequestNetworkSettings requestNetworkSettings:
                    Console.WriteLine($"RequestNetworkSettings: protocol={requestNetworkSettings.ProtocolVersion}");
                    session.protocolVersion = requestNetworkSettings.ProtocolVersion;
                    client.ProtocolVersion = requestNetworkSettings.ProtocolVersion;
                    SendRequestNetworkSettings(remoteEp, requestNetworkSettings.ProtocolVersion);
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
                    if (!_loginRequested)
                    {
                        SendLogin(remoteEp, client.ProtocolVersion, authToken);
                        _loginRequested = true;
                    }
                    break;
                case ServerToClientHandshake serverToClientHandshake:
                    Console.WriteLine($"ServerToClientHandshake received ({serverToClientHandshake.JWT.Length} chars).");
                    SendClientToServerHandshake(remoteEp);
                    break;
                case PlayStatus playStatus:
                    Console.WriteLine($"PlayStatus: {playStatus.Status}");
                    if (playStatus.Status == 0)
                    {
                        client.ConnectionState = 2;
                        Console.WriteLine("Login success.");
                    }
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
                else if (client.ConnectionState == 2)
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
            ClientId = ClientGuid
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

    private static void SendRequestNetworkSettings(IPEndPoint remoteEp, int protocolVersion)
    {
        var encoder = PacketEncoderPool.Get(remoteEp);
        var packet = new RequestNetworkSettings
        {
            ProtocolVersion = protocolVersion
        };
        packet.EncodePacket(encoder);
    }

    private static void SendLogin(IPEndPoint remoteEp, int protocolVersion, string? authToken)
    {
        if (string.IsNullOrWhiteSpace(authToken))
        {
            Console.WriteLine($"Missing {AuthTokenEnv}. Set it to the Minecraft/Xbox login token first.");
            _running = false;
            return;
        }

        Console.WriteLine("Sending Login.");
        var encoder = PacketEncoderPool.Get(remoteEp);
        var packet = new Login
        {
            ProtocolVersion = protocolVersion,
            Request = CreateLoginRequest(authToken)
        };
        packet.EncodePacket(encoder);
    }

    private static void SendClientToServerHandshake(IPEndPoint remoteEp)
    {
        var encoder = PacketEncoderPool.Get(remoteEp);
        var packet = new ClientToServerHandshake();
        packet.EncodePacket(encoder);
    }

    private static byte[] CreateLoginRequest(string authToken)
    {
        var loginJson = new LoginJson
        {
            AuthenticationType = (int)LoginHandler.AuthenticationType.Full,
            Certificate = $"{{\"chain\":[\"{JWT.CreateJWTchain()}\"]}}",
            Token = authToken
        };

        string jsonPart = Newtonsoft.Json.JsonConvert.SerializeObject(loginJson);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonPart);

        string tokenJWT = JWT.CreateJWTtoken();
        byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenJWT);

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write((uint)jsonBytes.Length);
        writer.Write(jsonBytes);
        writer.Write((uint)tokenBytes.Length);
        writer.Write(tokenBytes);

        return ms.ToArray();
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

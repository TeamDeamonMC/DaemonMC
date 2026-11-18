using System.Collections.Concurrent;
using System.Net;
using BasaltBE.NetherNet;
using BasaltBE.NetherNet.Enums;
using BasaltBE.NetherNet.Types;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.NetherNet
{
    public static class NetherNetTransport
    {
        public static bool Enabled { get; set; } = true;
        public static ushort Port { get; set; } = 19132;

        private static readonly ConcurrentDictionary<NethernetPeer, NetherNetEndPoint> endPoints = new();
        private static NethernetServer? server;

        public static bool Running => server != null;

        public static void Start()
        {
            if (!Enabled || server != null) return;

            var options = new NethernetServerOptions
            {
                Advertisement = new ServerAdvertisement
                {
                    Name = DaemonMC.Servername,
                    Protocol = Info.ProtocolVersion.Last(),
                    Version = Info.Version,
                    Level = DaemonMC.Worldname,
                    MaxPlayers = int.TryParse(DaemonMC.MaxOnline, out int max) ? max : 10,
                    GameType = DaemonMC.GameMode,
                    AcceptsOnlineAuth = true,
                    AcceptsSelfSignedAuth = !LoginHandler.XboxAuth,
                },
                HttpAddress = new IPEndPoint(IPAddress.Any, Port),
            };

            try
            {
                var nethernet = new NethernetServer(options);
                nethernet.Start(OnMessage);
                server = nethernet;
                Log.info($"NetherNet listening on port {Port} (TCP signaling) and 7551 (UDP LAN discovery)");
            }
            catch (Exception e)
            {
                Log.error($"NetherNet failed to start, only RakNet clients will be able to join: {e.Message}");
            }
        }

        public static void Stop()
        {
            var nethernet = Interlocked.Exchange(ref server, null);
            if (nethernet == null) return;
            try
            {
                nethernet.Dispose();
            }
            catch (Exception e)
            {
                Log.warn($"NetherNet shutdown error: {e.Message}");
            }
            endPoints.Clear();
        }

        public static void Send(NetherNetEndPoint ep, byte[] batch)
        {
            var nethernet = server;
            if (nethernet == null) return;
            try
            {
                nethernet.Send(ep.Peer, batch, MessageReliability.Reliable);
                Server.DatGrOut++;
            }
            catch (PeerDisconnectedException)
            {
                Log.debug($"Tried to send data to disconnected NetherNet peer {ep}");
            }
            catch (Exception e)
            {
                Log.warn($"NetherNet send to {ep} failed: {e.Message}");
            }
        }

        public static void Disconnect(NetherNetEndPoint ep)
        {
            var nethernet = server;
            if (nethernet == null) return;
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);
                try
                {
                    nethernet.Disconnect(ep.Peer);
                }
                catch (ObjectDisposedException) { }
            });
        }

        private static void OnMessage(NethernetMessage message)
        {
            if (message.Connected)
            {
                var ep = new NetherNetEndPoint(message.Peer);
                endPoints[message.Peer] = ep;
                RakSessionManager.createSession(ep);
                Log.debug($"[NetherNet] New connection: {ep}", ConsoleColor.DarkYellow);
                return;
            }

            if (message.Disconnected)
            {
                if (endPoints.TryRemove(message.Peer, out var closed) && RakSessionManager.sessions.TryGetValue(closed, out var session))
                {
                    lock (Server.PacketLock)
                    {
                        if (session.EntityID != 0)
                        {
                            Server.RemovePlayer(session.EntityID);
                        }
                        RakSessionManager.deleteSession(closed, "Disconnected");
                    }
                }
                return;
            }

            if (message.Payload.IsEmpty || !endPoints.TryGetValue(message.Peer, out var clientEp))
            {
                return;
            }

            if (!RakSessionManager.sessions.TryGetValue(clientEp, out var current))
            {
                return; // session already closed by the server
            }

            Server.DatGrIn++;
            current.LastPingTime = DateTime.UtcNow;
            lock (Server.PacketLock)
            {
                BedrockPacketDecoder.BedrockDecoder(PacketDecoderPool.Get(message.Payload.ToArray(), clientEp));
            }
        }
    }
}

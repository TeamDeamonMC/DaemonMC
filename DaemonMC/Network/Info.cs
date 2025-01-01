namespace DaemonMC.Network
{
    public class Info
    {
        public static string version = "1.21.50";
        public static int[] protocolVersion = [748, 766];

        public static int v1_21_40 = 748;
        public static int v1_21_50 = 766;

        public enum Bedrock
        {
            Example = -1,
            Login = 1,
            PlayStatus = 2,
            ServerToClientHandshake = 3,
            Disconnect = 5,
            ResourcePacksInfo = 6,
            ResourcePackStack = 7,
            ResourcePackClientResponse = 8,
            TextMessage = 9,
            StartGame = 11,
            AddPlayer = 12,
            MovePlayer = 19,
            UpdateAttributes = 29,
            Interact = 33,
            SetActorData = 39,
            SetActorMotion = 40,
            LevelChunk = 58,
            RequestChunkRadius = 69,
            ChunkRadiusUpdated = 70,
            MoveActorDelta = 111,
            NetworkChunkPublisherUpdate = 121,
            BiomeDefinitionList = 122,
            ClientCacheStatus = 129,
            NetworkSettings = 143,
            PlayerAuthInput = 144,
            CreativeContent = 145,
            PacketViolationWarning = 156,
            RequestNetworkSettings = 193,
            ServerboundLoadingScreen = 312
        }

        public enum RakNet
        {
            ConnectedPing = 0,                    //0x00
            UnconnectedPing = 1,                  //0x01
            ConnectedPong = 3,                    //0x03
            OpenConnectionRequest1 = 5,           //0x05
            OpenConnectionReply1 = 6,             //0x06
            OpenConnectionRequest2 = 7,           //0x07
            OpenConnectionReply2 = 8,             //0x08
            ConnectionRequest = 9,                //0x09
            ConnectionRequestAccepted = 16,       //0x10
            NewIncomingConnection = 19,           //0x13
            Disconnect = 21,                      //0x15
            UnconnectedPong = 28,                 //0x1c
            NACK = 160,                           //0xa0
            ACK = 192,                            //0xc0
        }
    }
}

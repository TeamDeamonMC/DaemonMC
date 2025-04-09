namespace DaemonMC.Network
{
    public class Info
    {
        public static string Version = "1.21.70";
        public static int[] ProtocolVersion = [748, 766, 776, 786, /*800*/];

        public static int v1_21_40 = 748;
        public static int v1_21_50 = 766;
        public static int v1_21_60 = 776;
        public static int v1_21_70 = 786;
        public static int v1_21_80 = 800;

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
            SetTime = 10,
            StartGame = 11,
            AddPlayer = 12,
            AddActor = 13,
            RemoveActor = 14,
            MovePlayer = 19,
            UpdateBlock = 21,
            LevelEvent = 25,
            MobEffect = 28,
            UpdateAttributes = 29,
            InventoryTransaction = 30,
            Interact = 33,
            SetActorData = 39,
            SetActorMotion = 40,
            Animate = 44,
            InventorySlot = 50,
            LevelChunk = 58,
            SetPlayerGameType = 62,
            PlayerList = 63,
            RequestChunkRadius = 69,
            ChunkRadiusUpdated = 70,
            GameRulesChanged = 72,
            AvailableCommands = 76,
            CommandRequest = 77,
            ResourcePackDataInfo = 82,
            ResourcePackChunkData = 83,
            ResourcePackChunkRequest = 84,
            TransferPlayer = 85,
            SetTitle = 88,
            PlayerSkin = 93,
            ModalFormRequest = 100,
            ModalFormResponse = 101,
            MoveActorDelta = 111,
            SetLocalPlayerAsInitialized = 113,
            NetworkChunkPublisherUpdate = 121,
            BiomeDefinitionList = 122,
            ClientCacheStatus = 129,
            Emote = 138,
            NetworkSettings = 143,
            PlayerAuthInput = 144,
            CreativeContent = 145,
            EmoteList = 152,
            PacketViolationWarning = 156,
            AnimateEntity = 158,
            ItemRegistry = 162,
            SyncActorProperty = 165,
            UpdateAbilities = 187,
            UpdateAdventureSettings = 188,
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

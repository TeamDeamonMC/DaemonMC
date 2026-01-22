namespace DaemonMC.Network
{
    public class Info
    {
        public static string Version = "1.26.0";

        public static int v1_21_90 = 818;
        public static int v1_21_93 = 819;
        public static int v1_21_100 = 827;
        public static int v1_21_111 = 844;
        public static int v1_21_120 = 859;
        public static int v1_21_124 = 860;
        public static int v1_21_130 = 898;
        public static int v1_26_0 = 924;

        public static int[] ProtocolVersion = [v1_21_90, v1_21_93, v1_21_100, v1_21_111, v1_21_120, v1_21_124, v1_21_130, v1_26_0];

        public enum Bedrock
        {
            Example = -1,
            Login = 1,
            PlayStatus = 2,
            ServerToClientHandshake = 3,
            ClientToServerHandshake = 4,
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
            MobEquipment = 31,
            MobArmorEquipment = 32,
            PlayerAction = 36,
            SetPlayerInventoryOptions = 307,
            SetActorData = 39,
            SetActorMotion = 40,
            Animate = 44,
            ContainerOpen = 46,
            ContainerClose = 47,
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
            LevelSoundEventPacket = 123,
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
            SetHud = 308,
            ServerboundLoadingScreen = 312,
            ClientMovementPredictionSync = 322
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
            GamePacket = 254,                     //0xfe
        }
    }
}

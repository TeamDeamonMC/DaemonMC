using System.Numerics;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerAuthInput : Packet
    {
        public override int Id => (int) Info.Bedrock.PlayerAuthInput;

        public Vector2 Rotation { get; set; } = new Vector2();
        public Vector3 Position { get; set; } = new Vector3();
        public Vector2 MoveVector { get; set; } = new Vector2();
        public float HeadRotation { get; set; } = 0;
        public List<AuthInputData> InputData { get; set; } = new List<AuthInputData>();
        public int InputMode { get; set; } = 0;
        public int PlayMode { get; set; } = 0;
        public int InteractionModel { get; set; } = 0;
        public Vector2 InteractRotation { get; set; } = new Vector2();
        public long Tick { get; set; } = 0;
        public Vector3 PosDelta { get; set; } = new Vector3();
        public Vector2 AnalogMove { get; set; } = new Vector2();
        public Vector3 CameraOrientation { get; set; } = new Vector3();
        public Vector2 RawMove { get; set; } = new Vector2();

        public int ClientRequestID { get; set; } = 0;
        public List<Actions> ActionsData { get; set; } = new List<Actions>();
        public List<string> StringsToFilter { get; set; } = new List<string>();
        public int StringsToFilterOrigin { get; set; } = 0;

        public PlayerBlockAction BlockAction { get; set; } = new PlayerBlockAction();

        public Vector2 VehicleRotation { get; set; } = new Vector2();
        public long ClientPredictedVehicle { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            Rotation = decoder.ReadVec2();
            Position = decoder.ReadVec3();
            MoveVector = decoder.ReadVec2();
            HeadRotation = decoder.ReadFloat();
            InputData = decoder.Read<AuthInputData>();
            InputMode = decoder.ReadVarInt();
            PlayMode = decoder.ReadVarInt();
            InteractionModel = decoder.ReadVarInt();
            InteractRotation = decoder.ReadVec2();
            Tick = decoder.ReadVarLong();
            PosDelta = decoder.ReadVec3();
            if (InputData.Contains(AuthInputData.PerformItemInteraction))
            {
                //todo
            }
            if (InputData.Contains(AuthInputData.PerformItemStackRequest))
            {
                ClientRequestID = decoder.ReadVarInt();
                ActionsData = decoder.ReadActions();
                StringsToFilter = decoder.ReadStringList();
                StringsToFilterOrigin = decoder.ReadInt();
            }
            if (InputData.Contains(AuthInputData.PerformBlockActions))
            {
                BlockAction = decoder.ReadBlockActions();
            }
            if (InputData.Contains(AuthInputData.IsInClientPredictedVehicle))
            {
                VehicleRotation = decoder.ReadVec2();
                ClientPredictedVehicle = decoder.ReadSignedVarLong();
            }
            AnalogMove = decoder.ReadVec2();
            CameraOrientation = decoder.ReadVec3();
            RawMove = decoder.ReadVec2();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

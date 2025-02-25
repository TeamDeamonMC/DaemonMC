using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerAuthInput : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PlayerAuthInput;

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
            //ItemUse =
            //ItemStack =
            //BlockActions = 
            //PredictedVehicle =
            AnalogMove = decoder.ReadVec2();
            CameraOrientation = decoder.ReadVec3();
            if (decoder.protocolVersion >= Info.v1_21_50)
            {
                RawMove = decoder.ReadVec2();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

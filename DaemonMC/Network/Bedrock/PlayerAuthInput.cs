using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerAuthInput : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PlayerAuthInput;

        public Vector2 Rotation = new Vector2();
        public Vector3 Position = new Vector3();
        public Vector2 MoveVector = new Vector2();
        public float HeadRotation = 0;
        public List<AuthInputData> InputData = new List<AuthInputData>();
        public int InputMode = 0;
        public int PlayMode = 0;
        public int InteractionModel = 0;
        public Vector2 InteractRotation = new Vector2();
        public long Tick = 0;
        public Vector3 PosDelta = new Vector3();
        public Vector2 AnalogMove = new Vector2();
        public Vector3 CameraOrientation = new Vector3();
        public Vector2 RawMove = new Vector2();

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
            RawMove = decoder.ReadVec2();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

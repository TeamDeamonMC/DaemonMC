using System.Numerics;
using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;
using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

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

        public Transaction? ItemUseTransaction { get; set; } = null;

        public int ClientRequestID { get; set; } = 0;
        public List<Actions> ActionsData { get; set; } = new List<Actions>();
        public List<string> StringsToFilter { get; set; } = new List<string>();
        public int StringsToFilterOrigin { get; set; } = 0;

        public List<PlayerBlockAction> BlockActions { get; set; } = new List<PlayerBlockAction>();
        public PlayerBlockAction BlockAction { get; set; } = new PlayerBlockAction();

        public Vector2 VehicleRotation { get; set; } = new Vector2();
        public long ClientPredictedVehicle { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            bool v1_26_40 = decoder.protocolVersion >= Info.v1_26_40;

            Rotation = decoder.ReadVec2();
            Position = decoder.ReadVec3();
            MoveVector = decoder.ReadVec2();
            HeadRotation = decoder.ReadFloat();
            if (v1_26_40)
            {
                InputData = ReadInputData(decoder);
            }
            else
            {
                InputData = decoder.ReadLegacy<AuthInputData>();
            }
            InputMode = decoder.ReadVarInt();
            PlayMode = decoder.ReadVarInt();
            InteractionModel = v1_26_40 ? decoder.ReadSignedVarInt() : decoder.ReadVarInt();
            InteractRotation = decoder.ReadVec2();
            Tick = decoder.ReadVarLong();
            PosDelta = decoder.ReadVec3();

            try
            {
                if (v1_26_40 ? decoder.ReadPresence() : InputData.Contains(AuthInputData.PerformItemInteraction))
                {
                    ItemUseTransaction = ReadItemUseTransaction(decoder, v1_26_40);
                }
                if (v1_26_40 ? decoder.ReadPresence() : InputData.Contains(AuthInputData.PerformItemStackRequest))
                {
                    var request = decoder.ReadItemStackRequest();
                    ClientRequestID = request.RequestId;
                    ActionsData = request.Actions;
                    StringsToFilter = request.StringToFilter;
                    StringsToFilterOrigin = request.StringToFilterOrigin;
                }
            }
            catch (NotSupportedException e)
            {
                Log.warn($"PlayerAuthInput: {e.Message}");
                return;
            }
            if (v1_26_40 ? decoder.ReadPresence() : InputData.Contains(AuthInputData.PerformBlockActions))
            {
                BlockActions = decoder.ReadBlockActions();
                BlockAction = BlockActions.LastOrDefault() ?? new PlayerBlockAction();
            }
            if (v1_26_40)
            {
                if (decoder.ReadPresence())
                {
                    VehicleRotation = decoder.ReadVec2();
                }
                if (decoder.ReadPresence())
                {
                    ClientPredictedVehicle = decoder.ReadSignedVarLong();
                }
            }
            else if (InputData.Contains(AuthInputData.IsInClientPredictedVehicle))
            {
                VehicleRotation = decoder.ReadVec2();
                ClientPredictedVehicle = decoder.ReadSignedVarLong();
            }
            AnalogMove = decoder.ReadVec2();
            CameraOrientation = decoder.ReadVec3();
            RawMove = decoder.ReadVec2();
        }

        private static List<AuthInputData> ReadInputData(PacketDecoder decoder)
        {
            var result = new List<AuthInputData>();
            if (decoder.protocolVersion < Info.v1_26_50 && !decoder.ReadBool())
            {
                return result;
            }
            int count = decoder.ReadVarInt();
            for (int i = 0; i < count; i++)
            {
                result.Add((AuthInputData)decoder.ReadSignedVarInt());
            }
            return result;
        }

        private static Transaction ReadItemUseTransaction(PacketDecoder decoder, bool newShape)
        {
            var transaction = new Transaction();
            transaction.Type = TransactionType.ItemUseTransaction;
            transaction.LegacyRequestId = decoder.ReadSignedVarInt();
            transaction.LegacySlots = decoder.ReadLegacySlots(transaction.LegacyRequestId, newShape);
            if (!newShape || decoder.protocolVersion >= Info.v1_26_50 || (decoder.ReadBool() && decoder.ReadBool()))
            {
                transaction.Actions = decoder.ReadTransactionActions(newShape);
            }
            decoder.ReadItemUseData(transaction, newShape);
            return transaction;
        }

        protected override void Encode(PacketEncoder encoder)
        {
            bool v1_26_40 = encoder.protocolVersion >= Info.v1_26_40;
            bool v1_26_50 = encoder.protocolVersion >= Info.v1_26_50;

            encoder.WriteVec2(Rotation);
            encoder.WriteVec3(Position);
            encoder.WriteVec2(MoveVector);
            encoder.WriteFloat(HeadRotation);
            if (v1_26_40)
            {
                if (!v1_26_50)
                {
                    encoder.WriteBool(InputData.Count > 0);
                }
                if (v1_26_50 || InputData.Count > 0)
                {
                    encoder.WriteVarInt(InputData.Count);
                    foreach (var input in InputData)
                    {
                        encoder.WriteSignedVarInt((int)input);
                    }
                }
            }
            else
            {
                ulong flags = 0;
                foreach (var input in InputData)
                {
                    flags |= 1UL << (int)input;
                }
                encoder.WriteVarLong(flags);
            }
            encoder.WriteVarInt(InputMode);
            encoder.WriteVarInt(PlayMode);
            if (v1_26_40)
            {
                encoder.WriteSignedVarInt(InteractionModel);
            }
            else
            {
                encoder.WriteVarInt(InteractionModel);
            }
            encoder.WriteVec2(InteractRotation);
            encoder.WriteVarLong(Tick);
            encoder.WriteVec3(PosDelta);
            if (v1_26_40)
            {
                for (int i = 0; i < 5; i++)
                {
                    encoder.WriteBool(false);
                    if (!v1_26_50)
                    {
                        encoder.WriteBool(false);
                    }
                }
            }
            encoder.WriteVec2(AnalogMove);
            encoder.WriteVec3(CameraOrientation);
            encoder.WriteVec2(RawMove);
        }
    }
}

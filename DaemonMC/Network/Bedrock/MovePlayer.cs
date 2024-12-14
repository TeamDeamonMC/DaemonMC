using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MovePlayerPacket
    {
        public long actorRuntimeId { get; set; }
        public Vector3 position { get; set; }
        public Vector2 rotation { get; set; }
        public float YheadRotation { get; set; }
        public byte positionMode { get; set; }
        public bool isOnGround { get; set; }
        public long vehicleRuntimeId { get; set; }
        public long tick { get; set; }
    }

    public class MovePlayer
    {
        public const int id = 19;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new MovePlayerPacket
            {
                actorRuntimeId = decoder.ReadVarLong(),
                position = decoder.ReadVec3(),
                rotation = decoder.ReadVec2(),
                YheadRotation = decoder.ReadFloat(),
                positionMode = decoder.ReadByte(),
                isOnGround = decoder.ReadBool(),
                vehicleRuntimeId = decoder.ReadVarLong()
            };

            BedrockPacketProcessor.MovePlayer(packet);
        }

        public static void Encode(MovePlayerPacket fields)
        {

        }
    }
}

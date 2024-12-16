using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MovePlayer
    {
        public Info.Bedrock id = Info.Bedrock.MovePlayer;

        public long actorRuntimeId = 0;
        public Vector3 position = new Vector3();
        public Vector2 rotation = new Vector2();
        public float YheadRotation = 0;
        public byte positionMode = 0;
        public bool isOnGround = false;
        public long vehicleRuntimeId = 0;
        public long tick = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new MovePlayer
            {
                actorRuntimeId = decoder.ReadVarLong(),
                position = decoder.ReadVec3(),
                rotation = decoder.ReadVec2(),
                YheadRotation = decoder.ReadFloat(),
                positionMode = decoder.ReadByte(),
                isOnGround = decoder.ReadBool(),
                vehicleRuntimeId = decoder.ReadVarLong()
            };

            decoder.player.PacketEvent_MovePlayer(packet);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}

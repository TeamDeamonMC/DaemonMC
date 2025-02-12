using System.Numerics;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class PlayerAuthInput
    {
        public Info.Bedrock id = Info.Bedrock.PlayerAuthInput;

        public Vector2 Rotation = new Vector2();
        public Vector3 Position = new Vector3();
        public Vector2 MoveVector = new Vector2();
        public float HeadRotation = 0;
        public List<AuthInputData> InputData = new List<AuthInputData>();

        public void Decode(PacketDecoder decoder)
        {
            var packet = new PlayerAuthInput
            {
                Rotation = decoder.ReadVec2(),
                Position = decoder.ReadVec3(),
                MoveVector = decoder.ReadVec2(),
                HeadRotation = decoder.ReadFloat(),
                InputData = decoder.Read<AuthInputData>()

            };

            decoder.player.PacketEvent_PlayerAuthInput(packet);
        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.handlePacket();
        }
    }
}

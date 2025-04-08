using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class SyncActorProperty : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.SyncActorProperty;

        public NbtCompound Data { get; set; } = new NbtCompound("");

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteCompoundTag(Data);
        }
    }
}

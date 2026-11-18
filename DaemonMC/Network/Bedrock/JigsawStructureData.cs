using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class JigsawStructureData : Packet
    {
        public override int Id => (int) Info.Bedrock.JigsawStructureData;

        public NbtCompound StructureData { get; set; } = CreateEmpty();

        public static NbtCompound CreateEmpty()
        {
            return new NbtCompound("")
            {
                new NbtList("processors", NbtTagType.Compound),
                new NbtList("template_pools", NbtTagType.Compound),
                new NbtList("jigsaws", NbtTagType.Compound),
                new NbtList("structure_sets", NbtTagType.Compound),
            };
        }

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteCompoundTag(StructureData);
        }
    }
}

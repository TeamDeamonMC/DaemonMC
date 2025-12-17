namespace DaemonMC.Network.Bedrock
{
    public class SetPlayerInventoryOptions : Packet
    {
        public override int Id => (int) Info.Bedrock.SetPlayerInventoryOptions;

        public int LeftTab { get; set; } = 0;
        public int RightTab { get; set; } = 0;
        public bool Filtering { get; set; } = false;
        public int InventoryLayout { get; set; } = 0;
        public int CraftingLayout { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            LeftTab = decoder.ReadVarInt();
            RightTab = decoder.ReadVarInt();
            Filtering = decoder.ReadBool();
            InventoryLayout = decoder.ReadVarInt();
            CraftingLayout = decoder.ReadVarInt();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(LeftTab);
            encoder.WriteVarInt(RightTab);
            encoder.WriteBool(Filtering);
            encoder.WriteVarInt(InventoryLayout);
            encoder.WriteVarInt(CraftingLayout);
        }
    }
}

namespace DaemonMC.Network.Bedrock
{
    public class ModalFormResponse : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ModalFormResponse;

        public int ID { get; set; } = 0;
        public string? Data { get; set; } = "";
        public byte? CancelReason { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            ID = decoder.ReadVarInt();
            Data = decoder.ReadOptional(decoder.ReadString);
            CancelReason = decoder.ReadOptional(decoder.ReadByte);
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

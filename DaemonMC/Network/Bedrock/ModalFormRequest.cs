namespace DaemonMC.Network.Bedrock
{
    public class ModalFormRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.ModalFormRequest;

        public int ID { get; set; } = 0;
        public string Data { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(ID);
            encoder.WriteString(Data);
        }
    }
}

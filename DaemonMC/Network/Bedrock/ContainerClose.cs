namespace DaemonMC.Network.Bedrock
{
    public class ContainerClose : Packet
    {
        public override int Id => (int) Info.Bedrock.ContainerClose;

        public byte ContainerId { get; set; } = 0;
        public byte ContainerType { get; set; } = 0;
        public bool ServerInit { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {
            ContainerId = decoder.ReadByte();
            ContainerType = decoder.ReadByte();
            ServerInit = decoder.ReadBool();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(ContainerId);
            encoder.WriteByte(ContainerType);
            encoder.WriteBool(ServerInit);
        }
    }
}

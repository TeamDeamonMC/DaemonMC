namespace DaemonMC.Network.Bedrock
{
    public class TransferPlayer : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.TransferPlayer;

        public string IpAddress { get; set; } = "";
        public ushort Port { get; set; } = 0;
        public bool Reload { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(IpAddress);
            encoder.WriteShort(Port);
            encoder.WriteBool(Reload);
        }
    }
}

namespace DaemonMC.Network.Bedrock
{
    public class ServerToClientHandshake : Packet
    {
        public override int Id => (int) Info.Bedrock.ServerToClientHandshake;

        public string JWT { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteString(JWT);
        }
    }
}

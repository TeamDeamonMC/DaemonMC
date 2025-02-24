namespace DaemonMC.Network.Bedrock
{
    public class CommandRequest : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.CommandRequest;

        public string Command { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {
            Command = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

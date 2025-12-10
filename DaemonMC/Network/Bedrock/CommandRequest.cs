namespace DaemonMC.Network.Bedrock
{
    public class CommandRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.CommandRequest;

        public string Command { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {
            Command = decoder.ReadString(); //todo add all fields
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

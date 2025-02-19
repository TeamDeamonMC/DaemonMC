namespace DaemonMC.Network.Bedrock
{
    public class CommandRequest
    {
        public Info.Bedrock id = Info.Bedrock.CommandRequest;

        public string Command = "";

        public void Decode(PacketDecoder decoder)
        {
            var packet = new CommandRequest
            {
                Command = decoder.ReadString(),
            };

            decoder.player.PacketEvent_CommandRequest(packet);
        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.handlePacket();
        }
    }
}

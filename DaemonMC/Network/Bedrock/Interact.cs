namespace DaemonMC.Network.Bedrock
{
    public class Interact
    {
        public Info.Bedrock id = Info.Bedrock.Interact;

        public byte action = 0;
        public long actorRuntimeId = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new Interact
            {
                action = decoder.ReadByte(),
                actorRuntimeId = decoder.ReadVarLong()
            };

            BedrockPacketProcessor.Interact(packet);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}

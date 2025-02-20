namespace DaemonMC.Network.Bedrock
{
    public class Interact : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Interact;

        public byte action = 0;
        public long actorRuntimeId = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            action = decoder.ReadByte();
            actorRuntimeId = decoder.ReadVarLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

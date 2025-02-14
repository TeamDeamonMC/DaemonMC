namespace DaemonMC.Network.Bedrock
{
    public class RemoveActor
    {
        public Info.Bedrock id = Info.Bedrock.RemoveActor;

        public long EntityId = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteSignedVarLong(EntityId);
            encoder.handlePacket();
        }
    }
}

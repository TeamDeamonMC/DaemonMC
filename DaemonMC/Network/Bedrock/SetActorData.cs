using DaemonMC.Network.Enumerations;
using DaemonMC.Utils;

namespace DaemonMC.Network.Bedrock
{
    public class SetActorData
    {
        public Info.Bedrock id = Info.Bedrock.SetActorData;

        public ulong EntityId = 0;
        public Dictionary<ActorData, Metadata> Metadata = new Dictionary<ActorData, Metadata>();

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong(EntityId);
            encoder.WriteMetadata(Metadata);
            encoder.WriteVarInt(0);
            encoder.WriteVarInt(0); //todo here
            encoder.WriteVarInt(0);
            encoder.handlePacket();
        }
    }
}

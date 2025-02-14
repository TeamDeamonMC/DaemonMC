using DaemonMC.Network.Enumerations;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class SetActorData
    {
        public Info.Bedrock id = Info.Bedrock.SetActorData;

        public long EntityId = 0;
        public Dictionary<ActorData, Metadata> Metadata = new Dictionary<ActorData, Metadata>();
        public long Tick = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong((ulong) EntityId);
            encoder.WriteMetadata(Metadata);
            encoder.WriteVarInt(0);
            encoder.WriteVarInt(0); //todo here
            encoder.WriteVarLong((ulong) Tick);
            encoder.handlePacket();
        }
    }
}

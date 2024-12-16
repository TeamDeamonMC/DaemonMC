namespace DaemonMC.Network.Bedrock
{
    public class NetworkSettings
    {
        public Info.Bedrock id = Info.Bedrock.NetworkSettings;

        public ushort compressionThreshold = 0;
        public ushort compressionAlgorithm = 0;
        public bool clientThrottleEnabled = false;
        public byte clientThrottleThreshold = 0;
        public float clientThrottleScalar = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteShort(compressionThreshold);
            encoder.WriteShort(compressionAlgorithm);
            encoder.WriteBool(clientThrottleEnabled);
            encoder.WriteByte(clientThrottleThreshold);
            encoder.WriteFloat(clientThrottleScalar);
            encoder.handlePacket();
        }
    }
}

namespace DaemonMC.Network.Bedrock
{
    public class NetworkSettings : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.NetworkSettings;

        public ushort compressionThreshold = 0;
        public ushort compressionAlgorithm = 0;
        public bool clientThrottleEnabled = false;
        public byte clientThrottleThreshold = 0;
        public float clientThrottleScalar = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteShort(compressionThreshold);
            encoder.WriteShort(compressionAlgorithm);
            encoder.WriteBool(clientThrottleEnabled);
            encoder.WriteByte(clientThrottleThreshold);
            encoder.WriteFloat(clientThrottleScalar);
        }
    }
}

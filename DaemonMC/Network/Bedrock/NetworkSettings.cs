namespace DaemonMC.Network.Bedrock
{
    public class NetworkSettings : Packet
    {
        public override int Id => (int) Info.Bedrock.NetworkSettings;

        public ushort CompressionThreshold { get; set; } = 0;
        public ushort CompressionAlgorithm { get; set; } = 0;
        public bool ClientThrottleEnabled { get; set; } = false;
        public byte ClientThrottleThreshold { get; set; } = 0;
        public float ClientThrottleScalar { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            CompressionThreshold = decoder.ReadShort();
            CompressionAlgorithm = decoder.ReadShort();
            ClientThrottleEnabled = decoder.ReadBool();
            ClientThrottleThreshold = decoder.ReadByte();
            ClientThrottleScalar = decoder.ReadFloat();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteShort(CompressionThreshold);
            encoder.WriteShort(CompressionAlgorithm);
            encoder.WriteBool(ClientThrottleEnabled);
            encoder.WriteByte(ClientThrottleThreshold);
            encoder.WriteFloat(ClientThrottleScalar);
        }
    }
}

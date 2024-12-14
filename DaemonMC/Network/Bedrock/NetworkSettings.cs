namespace DaemonMC.Network.Bedrock
{
    public class NetworkSettingsPacket
    {
        public ushort compressionThreshold { get; set; }
        public ushort compressionAlgorithm { get; set; }
        public bool clientThrottleEnabled { get; set; }
        public byte clientThrottleThreshold { get; set; }
        public float clientThrottleScalar { get; set; }
    }

    public class NetworkSettings
    {
        public static int id = 143;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(NetworkSettingsPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteShort(fields.compressionThreshold);
            encoder.WriteShort(fields.compressionAlgorithm);
            encoder.WriteBool(fields.clientThrottleEnabled);
            encoder.WriteByte(fields.clientThrottleThreshold);
            encoder.WriteFloat(fields.clientThrottleScalar);
            encoder.handlePacket();
        }
    }
}

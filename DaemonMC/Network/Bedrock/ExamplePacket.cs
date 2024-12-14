namespace DaemonMC.Network.Bedrock
{
    public class ExamplePacket
    {
        public int variable { get; set; }
    }

    public class Example
    {
        public static int id = 0;
        public static void Decode(PacketDecoder decoder)
        {

        }

        public static void Encode(ExamplePacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.handlePacket();
        }
    }
}

namespace DaemonMC.Network.Bedrock
{
    public class CreativeContentPacket
    {

    }

    public class CreativeContent
    {
        public static int id = 145;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(CreativeContentPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteVarInt(0);
            encoder.handlePacket();
        }
    }
}

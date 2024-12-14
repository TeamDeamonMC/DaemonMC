namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackStackPacket
    {
        public bool forceTexturePack { get; set; }
    }

    public class ResourcePackStack
    {
        public static int id = 7;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(ResourcePackStackPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteBool(fields.forceTexturePack);
            encoder.WriteVarInt(0); //add-on list
            encoder.WriteVarInt(0); //texture pack list
            encoder.WriteString(Info.version);
            encoder.WriteInt(0); //experiments
            encoder.WriteBool(false); //experiments was on
            encoder.WriteBool(false); //editor packs
            encoder.handlePacket();
        }
    }
}

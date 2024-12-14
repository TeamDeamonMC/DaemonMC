using System.Diagnostics;
using System.Text;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class DisconnectPacket
    {
        public string message { get; set; }
    }

    public class Disconnect
    {
        public static int id = 5;
        public static void Decode(byte[] buffer)
        {

        }

        public static void Encode(DisconnectPacket fields, PacketEncoder encoder)
        {
            encoder.WriteVarInt(id);
            encoder.WriteVarInt(0);
            encoder.WriteBool(false);
            encoder.WriteString(fields.message);
            encoder.WriteString("");
            encoder.handlePacket();
        }
    }
}

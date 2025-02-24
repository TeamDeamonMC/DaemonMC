using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class AvailableCommands : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.AvailableCommands;

        public List<string> EnumValues { get; set; } = new List<string>();
        public List<string> ChainedSubcommandValues { get; set; } = new List<string>();
        public List<string> PostFixes { get; set; } = new List<string>();
        public List<Command> Commands { get; set; } = new List<Command>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder) //todo looks terrible
        {
            encoder.WriteVarInt(EnumValues.Count);
            foreach (var value in EnumValues)
            {
                encoder.WriteString(value);
            }
            encoder.WriteVarInt(ChainedSubcommandValues.Count);
            foreach (var value in ChainedSubcommandValues)
            {
                encoder.WriteString(value);
            }
            encoder.WriteVarInt(PostFixes.Count);
            foreach (var value in PostFixes)
            {
                encoder.WriteString(value);
            }
            encoder.WriteVarInt(0);//enum data
            encoder.WriteVarInt(0);//ChainedSubcommand data

            encoder.WriteVarInt(Commands.Count);
            foreach (var command in Commands)
            {
                encoder.WriteString(command.Name);
                encoder.WriteString(command.Description);
                encoder.WriteShort((ushort)command.Flags);
                encoder.WriteByte(command.Permission);
                encoder.WriteInt(command.AliasEnum);
                encoder.WriteVarInt(command.ChainedSubcommandIndex.Count());
                foreach (var index in command.ChainedSubcommandIndex)
                {
                    encoder.WriteShort((ushort)index);
                }
                encoder.WriteVarInt(0);
            }

            encoder.WriteVarInt(0);//soft enums
            encoder.WriteVarInt(0);//constraints
        }
    }
}

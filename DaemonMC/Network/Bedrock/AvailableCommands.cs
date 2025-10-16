using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class AvailableCommands : Packet
    {
        public override int Id => (int) Info.Bedrock.AvailableCommands;

        public List<string> EnumValues { get; set; } = new List<string>();
        public List<CommandEnum> Enums { get; set; } = new List<CommandEnum>();
        public List<string> ChainedSubcommandValues { get; set; } = new List<string>();
        public List<string> PostFixes { get; set; } = new List<string>();
        public List<Command> Commands { get; set; } = new List<Command>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteStringList(EnumValues);
            encoder.WriteStringList(ChainedSubcommandValues);
            encoder.WriteStringList(PostFixes);
            encoder.WriteVarInt(Enums.Count);
            foreach (var enumEntry in Enums)
            {
                encoder.WriteString(enumEntry.EnumName);
                encoder.WriteVarInt(enumEntry.Values.Count());
                foreach (var val in enumEntry.Values)
                {
                    encoder.WriteByte((byte)CommandManager.EnumValues.IndexOf(val));
                }
            }

            encoder.WriteVarInt(0);//ChainedSubcommand data

            encoder.WriteVarInt(Commands.Count);
            foreach (var command in Commands)
            {
                encoder.WriteString(command.Name);
                encoder.WriteString(command.Description);
                encoder.WriteShort((ushort)command.Flags);
                encoder.WriteByte(command.Permission);
                encoder.WriteInt(-1);
                encoder.WriteVarInt(command.ChainedSubcommandIndex.Count());
                foreach (var index in command.ChainedSubcommandIndex)
                {
                    encoder.WriteShort((ushort)index);
                }
                encoder.WriteVarInt(command.Overloads.Count());
                foreach (var overload in command.Overloads)
                {
                    encoder.WriteBool(false);
                    encoder.WriteVarInt(overload.Count());
                    foreach (var parameter in overload)
                    {
                        encoder.WriteString(parameter.Name);
                        encoder.WriteInt(CommandManager.GetSymbol(parameter.Type, Enums.FindIndex(p => p.Name == parameter.Name)));
                        encoder.WriteBool(parameter.Optional);
                        encoder.WriteByte(0);
                    }
                }
            }

            encoder.WriteVarInt(0);//soft enums
            encoder.WriteVarInt(0);//constraints
        }
    }
}

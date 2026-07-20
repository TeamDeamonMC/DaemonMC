using DaemonMC.Items;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class CreativeContent : Packet
    {
        public override int Id => (int) Info.Bedrock.CreativeContent;
        
        public List<CreativeItemGroup>? Groups { get; set; } = null;
        
        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (Groups == null)
            {
                encoder.WriteVarInt(0);
                encoder.WriteVarInt(0);
                return;
            }

            encoder.WriteVarInt(Groups.Count);

            for (var groupIndex = 0; groupIndex < Groups.Count; groupIndex++)
            {
                var group = Groups[groupIndex];
                encoder.WriteInt((int)group.Category);
                encoder.WriteString(group.Name);
                encoder.WriteItemInstance(group.Icon ?? ItemPalette.GetItem("minecraft:air"));
            }

            encoder.WriteVarInt(Groups.Sum(g => g.Items.Count));

            var creativeNetId = 0;

            for (int groupIndex = 0; groupIndex < Groups.Count; groupIndex++)
            {
                var group = Groups[groupIndex];

                foreach (var item in group.Items)
                {
                    creativeNetId++;
                    encoder.WriteVarInt(creativeNetId);
                    encoder.WriteItemInstance(item);
                    encoder.WriteVarInt(groupIndex);
                }
            }
        }
    }
}

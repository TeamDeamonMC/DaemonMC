using DaemonMC.Items;
using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class CreativeContent : Packet
    {
        public override int Id => (int) Info.Bedrock.CreativeContent;
        
        public List<CreativeItemGroup>? Groups { get; set; } = null;

        public Dictionary<Item, CreativeItemGroup>? Items { get; set; } = null;
        
        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (Groups == null || Items == null)
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
                encoder.WriteItem(group.Icon ?? ItemPalette.GetItem("minecraft:air"));
            }

            var creativeNetId = 0;

            encoder.WriteVarInt(Items.Count);

            foreach (var entry in Items)
            {
                creativeNetId++;
                encoder.WriteVarInt(creativeNetId);
                encoder.WriteItem(entry.Key);
                encoder.WriteVarInt(entry.Value?.GroupId ?? 0);
            }
        }
    }
}

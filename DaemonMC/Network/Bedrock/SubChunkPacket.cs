using DaemonMC.Level;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class SubChunkPacket : Packet
    {
        public override int Id => (int) Info.Bedrock.SubChunk;

        public int Dimension { get; set; } = 0;
        public int CenterX { get; set; } = 0;
        public int CenterY { get; set; } = 0;
        public int CenterZ { get; set; } = 0;
        public List<SubChunkEntry> Entries { get; set; } = new List<SubChunkEntry>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            if (encoder.protocolVersion >= Info.v1_26_40)
            {
                encoder.WriteBool(false);
                encoder.WriteSignedVarInt(Dimension);
                encoder.WriteInt(CenterX);
                encoder.WriteInt(CenterY);
                encoder.WriteInt(CenterZ);
                encoder.WriteVarInt(Entries.Count);

                foreach (var entry in Entries)
                {
                    encoder.WriteByte((byte)entry.OffsetX);
                    encoder.WriteByte((byte)entry.OffsetY);
                    encoder.WriteByte((byte)entry.OffsetZ);
                    encoder.WriteByte((byte)entry.Result);

                    bool hasData = entry.Payload.Length > 0;
                    encoder.WriteBool(hasData);
                    if (hasData)
                    {
                        encoder.WriteBytes(entry.Payload);
                    }

                    encoder.WriteHeightMap(encoder, entry.HeightMapType, entry.HeightMap);
                    encoder.WriteHeightMap(encoder, entry.RenderHeightMapType, entry.RenderHeightMap);

                    encoder.WriteBool(false);
                }
            }
            else
            {
                encoder.WriteBool(false);
                encoder.WriteSignedVarInt(Dimension);
                encoder.WriteSignedVarInt(CenterX);
                encoder.WriteSignedVarInt(CenterY);
                encoder.WriteSignedVarInt(CenterZ);
                encoder.WriteInt(Entries.Count);

                foreach (var entry in Entries)
                {
                    encoder.WriteByte((byte)entry.OffsetX);
                    encoder.WriteByte((byte)entry.OffsetY);
                    encoder.WriteByte((byte)entry.OffsetZ);
                    encoder.WriteByte((byte)entry.Result);
                    encoder.WriteBytes(entry.Payload);
                    encoder.WriteByte((byte)entry.HeightMapType);
                    if (entry.HeightMapType == HeightMapDataType.HasData)
                    {
                        encoder.WriteBytes((byte[])(Array)entry.HeightMap, false);
                    }
                    encoder.WriteByte((byte)entry.RenderHeightMapType);
                    if (entry.RenderHeightMapType == HeightMapDataType.HasData)
                    {
                        encoder.WriteBytes((byte[])(Array)entry.RenderHeightMap, false);
                    }
                }
            }
        }
    }
}

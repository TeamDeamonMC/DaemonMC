﻿using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class ResourcePackChunkRequest : Packet
    {
        public override int Id => (int) Info.Bedrock.ResourcePackChunkRequest;

        public string PackName { get; set; } = "";
        public int Chunk { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            PackName = decoder.ReadString();
            Chunk = decoder.ReadInt();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

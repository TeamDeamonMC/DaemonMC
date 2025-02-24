﻿using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class Interact : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Interact;

        public byte Action { get; set; } = 0;
        public long ActorRuntimeId { get; set; } = 0;
        public Vector3 InteractPosition { get; set; } = new Vector3();

        protected override void Decode(PacketDecoder decoder)
        {
            Action = decoder.ReadByte();
            ActorRuntimeId = decoder.ReadVarLong();
            if (Action == 3 || Action == 4)
            {
                InteractPosition = decoder.ReadVec3();
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}

using System.Numerics;
using DaemonMC.Network.Enumerations;
using fNbt;

namespace DaemonMC.Utils.Game
{
    public class Metadata
    {
        public object Value { get; set; } = new object();

        public Metadata(object value)
        {
            Value = value;
        }

        public EntityMetadataType Type => Value switch
        {
            byte => EntityMetadataType.Byte,
            short => EntityMetadataType.Short,
            int => EntityMetadataType.Int,
            float => EntityMetadataType.Float,
            string => EntityMetadataType.String,
            NbtCompound => EntityMetadataType.NbtCompound,
            long => EntityMetadataType.Long,
            Vector3 => EntityMetadataType.Vector3,
        };
    }
}

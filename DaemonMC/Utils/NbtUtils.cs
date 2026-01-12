using System;
using System.Collections.Generic;
using fNbt;

namespace DaemonMC.Utils
{
    public class NbtUtils
    {
        public static int IndexOf(List<NbtCompound> list, NbtCompound item)
        {
            var index = 0;

            var itemNbt = new NbtFile
            {
                BigEndian = false,
                UseVarInt = false,
                RootTag = item,
            };
            byte[] saveItemBuffer = itemNbt.SaveToBuffer(NbtCompression.None);
            var itemHash = Fnv1aHash.Hash32(saveItemBuffer);

            foreach (var value in list)
            {
                var nbt = new NbtFile
                {
                    BigEndian = false,
                    UseVarInt = false,
                    RootTag = value,
                };
                byte[] saveToBuffer = nbt.SaveToBuffer(NbtCompression.None);
                var blockHash = Fnv1aHash.Hash32(saveToBuffer);

                if (itemHash == blockHash)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static bool Contains(List<NbtCompound> list, NbtCompound item)
        {
            return IndexOf(list, item) == -1 ? false : true;
        }
    }
}

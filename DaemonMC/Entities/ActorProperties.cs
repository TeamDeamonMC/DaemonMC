using fNbt;

namespace DaemonMC.Entities
{
    public class ActorProperties
    {
        public static List<NbtCompound> PropertyData { get; set; } = new List<NbtCompound>();

        public static void RegisterProperty(string actorName, ActorProperty properties)
        {
            RegisterProperty(actorName, new List<ActorProperty> { properties });
        }

        public static void RegisterProperty(string actorName, List<ActorProperty> properties)
        {
            var rootCompound = new NbtCompound("");
            var propertiesList = new NbtList("properties");

            foreach (var property in properties)
            {
                var propertyCompound = new NbtCompound();
                PropertyType type = PropertyType.Unknown;

                switch (property.Property)
                {
                    case NbtCompound[] compoundArray:
                        type = PropertyType.Nbt;
                        foreach (var compound in compoundArray)
                            propertyCompound.Add(compound);
                        break;

                    case int[] intArray when intArray.Length >= 2:
                        type = PropertyType.Int;
                        propertyCompound.Add(new NbtInt("min", intArray[0]));
                        propertyCompound.Add(new NbtInt("max", intArray[1]));
                        break;

                    case string[] stringArray:
                        type = PropertyType.Enum;
                        var enumList = new NbtList("enum");
                        foreach (var str in stringArray)
                            enumList.Add(new NbtString(str));
                        propertyCompound.Add(enumList);
                        break;
                }

                propertyCompound.Add(new NbtString("name", property.propertyName));
                propertyCompound.Add(new NbtInt("type", (int)type));

                propertiesList.Add(propertyCompound);
            }

            rootCompound.Add(propertiesList);
            rootCompound.Add(new NbtString("type", actorName));

            PropertyData.Add(rootCompound);
        }
    }

    public class ActorProperty
    {
        public object Property;
        public string propertyName;

        public ActorProperty(object property, string name)
        {
            Property = property;
            propertyName = name;
        }
    }

    public enum PropertyType
    {
        Nbt = 0,
        Unknown = 1, // todo
        Int = 2,
        Enum = 3
    }
}

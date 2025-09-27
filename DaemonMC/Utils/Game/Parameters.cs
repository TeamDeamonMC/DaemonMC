using System.Numerics;

namespace DaemonMC.Utils.Game
{
    public abstract class Parameter
    {
        public string Name { get; set; } = "";
        public string EnumName { get; set; } = "Enum";
        public Type Type { get; set; }
        public string[] Values { get; set; }
        public bool Optional { get; set; }
    }

    public class IntP : Parameter
    {
        public IntP(string name, bool optional = false)
        {
            Name = name;
            Type = typeof(int);
            Optional = optional;
        }
    }

    public class FloatP : Parameter
    {
        public FloatP(string name, bool optional = false)
        {
            Name = name;
            Type = typeof(float);
            Optional = optional;
        }
    }

    public class StringP : Parameter
    {
        public StringP(string name, bool optional = false)
        {
            Name = name;
            Type = typeof(string);
            Optional = optional;
        }
    }

    public class PlayerP : Parameter
    {
        public PlayerP(string name, bool optional = false)
        {
            Name = name;
            Type = typeof(Player);
            Optional = optional;
        }
    }

    public class Vector3P : Parameter
    {
        public Vector3P(string name, bool optional = false)
        {
            Name = name;
            Type = typeof(Vector3);
            Optional = optional;
        }
    }

    public class JsonP : Parameter
    {
        public JsonP(string name, bool optional = false)
        {
            Name = name;
            Type = typeof(JsonP);
            Optional = optional;
        }
    }

    public class EnumP : Parameter
    {
        public EnumP(string name, string[] values, string enumName = "enum", bool optional = false)
        {
            Name = name;
            EnumName = enumName;
            Type = typeof(EnumP);
            Values = values;
            Optional = optional;
        }
    }

    public class BoolP : EnumP
    {
        public BoolP(string name, bool optional = false) : base(name, new string[] { "true", "false" }, "bool", optional) { }
    }

    public class ItemP : EnumP
    {
        public ItemP(string name, bool optional = false) : base(name, new string[] {}, "Item", optional) { } // client side enum, EEnumP for empty enums
    }
}

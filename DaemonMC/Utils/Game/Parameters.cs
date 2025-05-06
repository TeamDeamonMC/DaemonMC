using DaemonMC.Utils.Maths;

namespace DaemonMC.Utils.Game
{
    public abstract class Parameter
    {
        public string Name { get; set; } = "";
        public Type Type { get; set; }
        public string[] Values { get; set; }
    }

    public class IntP : Parameter
    {
        public IntP(string name)
        {
            Name = name;
            Type = typeof(int);
        }
    }

    public class FloatP : Parameter
    {
        public FloatP(string name)
        {
            Name = name;
            Type = typeof(float);
        }
    }

    public class StringP : Parameter
    {
        public StringP(string name)
        {
            Name = name;
            Type = typeof(string);
        }
    }

    public class PlayerP : Parameter
    {
        public PlayerP(string name)
        {
            Name = name;
            Type = typeof(Player);
        }
    }

    public class Vector3P : Parameter
    {
        public Vector3P(string name)
        {
            Name = name;
            Type = typeof(Vector3);
        }
    }

    public class EnumP : Parameter
    {
        public EnumP(string name, string[] values)
        {
            Name = name;
            Type = typeof(EnumP);
            Values = values;
        }
    }
}

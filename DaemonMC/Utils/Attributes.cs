namespace DaemonMC.Utils
{
    public class AttributeValue
    {
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float CurrentValue { get; set; }
        public float DefaultMinValue { get; set; }
        public float DefaultMaxValue { get; set; }
        public float DefaultValue { get; set; }
        public string Name { get; set; }

        public AttributeValue(string name, float minValue, float maxValue, float currentValue, float defaultMinValue, float defaultMaxValue, float defaultValue)
        {
            Name = name;
            MinValue = minValue;
            MaxValue = maxValue;
            CurrentValue = currentValue;
            DefaultMinValue = defaultMinValue;
            DefaultMaxValue = defaultMaxValue;
            DefaultValue = defaultValue;
        }
    }

    public class AttributesValues
    {
        public float MovementSpeed { get; set; }

        public AttributesValues(float movementSpeed)
        {
            MovementSpeed = movementSpeed;
        }

        public AttributeValue Movement_speed()
        {
            return new AttributeValue("minecraft:movement", 0, float.MaxValue, MovementSpeed, 0, float.MaxValue, MovementSpeed);
        }
    }
}

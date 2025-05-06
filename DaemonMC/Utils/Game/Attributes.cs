namespace DaemonMC.Utils.Game
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
        public float MovementSpeed { get; set; } = 0;
        public float UnderwaterMovementSpeed { get; set; } = 0;
        public float LavaMovementSpeed { get; set; } = 0;
        public float JumpStrength { get; set; } = 0;
        public float Health { get; set; } = 0;
        public float Hunger { get; set; } = 0;

        public AttributesValues()
        {

        }

        public AttributesValues(float movementSpeed = 0, float underwaterMovementSpeed = 0, float lavaMovementSpeed = 0, float jumpStrength = 0, float health = 0, float hunger = 0)
        {
            MovementSpeed = movementSpeed;
            UnderwaterMovementSpeed = underwaterMovementSpeed;
            LavaMovementSpeed = lavaMovementSpeed;
            JumpStrength = jumpStrength;
            Health = health;
            Hunger = hunger;
        }

        public AttributeValue Movement_speed()
        {
            return new AttributeValue("minecraft:movement", 0, float.MaxValue, MovementSpeed, 0, float.MaxValue, MovementSpeed);
        }
    }
}

namespace DaemonMC.Items
{
    public abstract class Item
    {
        public string Name { get; protected set; } = "minecraft:air";
        public short Id { get; protected set; } = 0;
    }
}

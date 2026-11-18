namespace DaemonMC.Blocks
{
    public class ChorusPlant : Block
    {
        public ChorusPlant()
        {
            Name = "minecraft:chorus_plant";

            BlastResistance = 0.4000000059604645;
            Brightness = 0;
            FlameEncouragement = 0;
            Flammability = 0;
            Friction = 0.6000000238418579;
            Hardness = 0.4000000059604645;
            Opacity = 1;

            States["minecraft:connection_down"] = (byte)0;
            States["minecraft:connection_east"] = (byte)0;
            States["minecraft:connection_north"] = (byte)0;
            States["minecraft:connection_south"] = (byte)0;
            States["minecraft:connection_up"] = (byte)0;
            States["minecraft:connection_west"] = (byte)0;
        }
    }
}

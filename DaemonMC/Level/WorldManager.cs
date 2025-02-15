using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Level
{
    public class WorldManager
    {
        public static void LoadWorlds(string worldDirectory)
        {
            if (!Directory.Exists(worldDirectory))
            {
                Log.warn($"{worldDirectory}/ not found. Creating new folder...");
            }
            Directory.CreateDirectory(worldDirectory);

            foreach (string file in Directory.GetFiles(worldDirectory))
            {
                Server.levels.Add(new World(Path.GetFileNameWithoutExtension(file)));
            }

            if(Server.levels.Count == 0)
            {
                Server.levels.Add(new World("Temp"));
            }

            var matchingLevels = Server.levels
                .Select((world, index) => new { World = world, Index = index })
                .Where(w => w.World.levelName == DaemonMC.defaultWorld)
                .ToList();

            if (matchingLevels.Count == 0)
            {
                Log.warn($"World name {DaemonMC.defaultWorld} specified in DaemonMC.yaml not found in Worlds directory.");
                Log.warn($"Check if DaemonMC.yaml 'spawnWorld' contains correct world name without extension.");
                Log.warn($"Players will be spawned in next found available world.");
            }
        }
    }
}

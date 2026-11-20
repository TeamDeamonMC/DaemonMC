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
                if (Path.GetExtension(file) == ".mcworld")
                {
                    Server.Levels.Add(new World(Path.GetFileNameWithoutExtension(file)));
                }
            }

            if (!Server.Levels.Any(w => w.LevelName == DaemonMC.DefaultWorld))
            {
                Server.Levels.Add(new World(DaemonMC.DefaultWorld));
            }
        }
    }
}

using DaemonMC.Utils.Text;
using System.Net;

namespace DaemonMC.Level
{
    public class Level
    {
        public bool temporary;
        public string levelName;
        public Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();
        public LevelDBInterface levelDB = new LevelDBInterface();

        public Level(string LevelName)
        {
            levelName = LevelName;
        }

        public void load()
        {
            var tempData = Path.Combine(Path.GetTempPath(), $"{levelName}.mcworld");
            if (Directory.Exists(tempData))
            {
                Directory.Delete(tempData, true);
            }
            if (File.Exists($"Worlds/{levelName}.mcworld"))
            {
                Log.info($"Loading world: {levelName}.mcworld");
            }
            else
            {
                Log.warn($"World Worlds/{levelName}.mcworld not found. Generating temporary flat world...");
                temporary = true;
            }
        }

        public long AddPlayer(Player player, IPEndPoint ep)
        {
            long id;

            if (Server.availableIds.Count > 0)
            {
                id = Server.availableIds.Dequeue();
            }
            else
            {
                id = Server.nextId++;
            }

            player.ep = ep;
            onlinePlayers.Add(id, player);
            Log.debug($"{player.Username} has been added to server players with EntityID {id}");
            return id;
        }

        public bool RemovePlayer(long id)
        {
            if (onlinePlayers.ContainsKey(id))
            {
                var player = GetPlayer(id);
                onlinePlayers.Remove(id);
                Server.availableIds.Enqueue(id);
                Log.debug($"Player {player.Username} with EntityID {id} has been removed from the server.");
                return true;
            }
            Log.error($"No player found with EntityID {id}");
            return false;
        }

        public Player GetPlayer(long id)
        {
            if (onlinePlayers.ContainsKey(id))
            {
                onlinePlayers.TryGetValue(id, out Player player);
                return player;
            }
            Log.error($"No player found with EntityID {id}");
            return null;
        }
    }
}

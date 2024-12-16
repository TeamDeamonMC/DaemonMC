using DaemonMC.Utils.Text;
using System.Net;

namespace DaemonMC.Level
{
    public class Level
    {
        public string levelName;
        public Dictionary<long, Player> onlinePlayers = new Dictionary<long, Player>();

        public Level(string LevelName)
        {
            levelName = LevelName;
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
                var username = GetPlayer(id).Username;
                onlinePlayers.Remove(id);
                Server.availableIds.Enqueue(id);
                Log.debug($"Player {username} with EntityID {id} has been removed from the server.");
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

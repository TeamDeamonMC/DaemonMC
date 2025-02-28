using System.Net;
using System.Reflection;
using DaemonMC.Entities;
using DaemonMC.Network;
using DaemonMC.Utils.Text;

namespace DaemonMC.Plugin.Plugin
{
    public class PluginManager
    {
        private static readonly List<Plugin> _plugins = new List<Plugin>();

        public static void LoadPlugins(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory))
            {
                Log.warn($"{pluginDirectory}/ not found. Creating new folder...");
            }
            Directory.CreateDirectory(pluginDirectory);


            foreach (var file in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                Log.info($"Loading plugin: {file}");
                Assembly assembly = Assembly.LoadFrom(file);

                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(Plugin).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        Plugin plugin = (Plugin)Activator.CreateInstance(type)!;
                        _plugins.Add(plugin);
                        plugin.OnLoad();
                    }
                }
            }
        }

        public static void UnloadPlugins()
        {
            Log.info($"Unloading plugins...");
            foreach (var plugin in _plugins)
            {
                plugin.OnUnload();
            }
            _plugins.Clear();
        }

        public static void PlayerJoined(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnPlayerJoined(player);
            }
        }

        public static void PlayerLeaved(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnPlayerLeaved(player);
            }
        }

        public static void PlayerMove(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnPlayerMove(player);
            }
        }

        public static bool PacketReceived(IPEndPoint ep, Packet packet)
        {
            foreach (var plugin in _plugins)
            {
                return plugin.OnPacketReceived(ep, packet);
            }
            return true;
        }

        public static bool PacketSent(IPEndPoint ep, Packet packet)
        {
            foreach (var plugin in _plugins)
            {
                return plugin.OnPacketSent(ep, packet);
            }
            return true;
        }

        public static void EntityAttack(Player player, Entity entity)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnEntityAttack(player, entity);
            }
        }

        public static void PlayerAttack(Player player, Player victim)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnPlayerAttack(player, victim);
            }
        }
    }
}

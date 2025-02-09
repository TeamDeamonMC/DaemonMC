using System.Reflection;
using DaemonMC.Utils.Text;

namespace DaemonMC.Plugin.Plugin
{
    public class PluginManager
    {
        private static readonly List<IPlayerPlugin> _plugins = new();

        public static void LoadPlugins(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory))
            {
                Log.warn($"{pluginDirectory}/ not found. Creating new folder...");
            }
                Directory.CreateDirectory(pluginDirectory);


            foreach (var file in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFrom(file);

                foreach (var type in assembly.GetTypes())
                {
                    Log.info($"Loading plugin: {file}");
                    if (typeof(IPlayerPlugin).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        IPlayerPlugin plugin = (IPlayerPlugin)Activator.CreateInstance(type)!;
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

        public static void OnPlayerJoin(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.OnPlayerJoin(player);
            }
        }
    }
}

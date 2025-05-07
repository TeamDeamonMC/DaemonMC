using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using DaemonMC.Entities;
using DaemonMC.Network;
using DaemonMC.Utils.Text;

namespace DaemonMC.Plugin.Plugin
{
    public class PluginManager
    {
        private static readonly List<LoadedPlugin> _plugins = new List<LoadedPlugin>();
        private static readonly Dictionary<string, DateTime> _reloadDelays = new Dictionary<string, DateTime>();
        private static readonly object _watcherLock = new();
        private static FileSystemWatcher? _watcher;

        public static void LoadPlugins(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory))
            {
                Log.warn($"{pluginDirectory}/ not found. Creating new folder...");
                Directory.CreateDirectory(pluginDirectory);
            }

            foreach (var file in Directory.GetFiles(pluginDirectory, "*.dll"))
            {
                LoadPlugin(file);
            }

            _watcher = new FileSystemWatcher(pluginDirectory, "*.dll")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };

            _watcher.Changed += (s, e) => HandlePluginChange(e.FullPath);
            _watcher.Created += (s, e) => HandlePluginChange(e.FullPath);
            _watcher.Deleted += (s, e) => HandlePluginRemoval(e.FullPath);
            _watcher.Renamed += (s, e) => HandlePluginRenamed(e.OldFullPath, e.FullPath);
        }

        public static void LoadPlugin(string filePath)
        {
            Log.info($"Loading plugin: {filePath}");

            var fullPath = Path.GetFullPath(filePath);
            var loadContext = new PluginLoadContext(fullPath);

            using var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            var assembly = loadContext.LoadFromStream(ms);

            foreach (var type in assembly.GetTypes())
            {
                if (typeof(Plugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    var pluginInstance = (Plugin)Activator.CreateInstance(type)!;
                    pluginInstance.OnLoad();

                    _plugins.Add(new LoadedPlugin
                    {
                        PluginInstance = pluginInstance,
                        LoadContext = loadContext,
                        Path = fullPath
                    });
                }
            }
        }

        public static void ReloadPlugin(string file)
        {
            UnloadPlugin(file);
            LoadPlugin(file);
        }

        public static void UnloadPlugins()
        {
            Log.info("Unloading plugins...");

            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.PluginInstance.OnUnload();
                    plugin.LoadContext.Unload();
                }
                catch (Exception ex)
                {
                    Log.error($"Failed to unload plugin: {ex.Message}");
                }
            }

            _plugins.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Log.info("Plugins successfully unloaded.");
        }

        public static void UnloadPlugin(string file)
        {
            var plugin = _plugins.FirstOrDefault(p => p.Path == Path.GetFullPath(file));
            if (plugin == null) return;

            plugin.PluginInstance.OnUnload();
            plugin.LoadContext.Unload();
            _plugins.Remove(plugin);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Log.info($"Plugin unloaded: {file}");
        }

        private static void HandlePluginChange(string fullPath)
        {
            lock (_watcherLock)
            {
                var now = DateTime.Now;
                if (_reloadDelays.TryGetValue(fullPath, out var lastTime))
                {
                    if ((now - lastTime).TotalMilliseconds < 500)
                        return;
                }
                _reloadDelays[fullPath] = now;
            }

            Task.Delay(500).ContinueWith(_ =>
            {
                try
                {
                    ReloadPlugin(fullPath);
                }
                catch (Exception ex)
                {
                    Log.error($"Failed to reload plugin: {ex}");
                }
            });
        }

        private static void HandlePluginRenamed(string oldPath, string newPath)
        {
            UnloadPlugin(oldPath);

            if (Path.GetExtension(newPath).Equals(".dll", StringComparison.OrdinalIgnoreCase))
            {
                Task.Delay(500).ContinueWith(_ => LoadPlugin(newPath));
            }
        }

        private static void HandlePluginRemoval(string path)
        {
            UnloadPlugin(path);
        }

        public static void PlayerJoined(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerJoined(player);
            }
        }

        public static void PlayerLeaved(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerLeaved(player);
            }
        }

        public static void PlayerMove(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerMove(player);
            }
        }

        public static bool PacketReceived(IPEndPoint ep, Packet packet)
        {
            foreach (var plugin in _plugins)
            {
                return plugin.PluginInstance.OnPacketReceived(ep, packet);
            }
            return true;
        }

        public static bool PacketSent(IPEndPoint ep, Packet packet)
        {
            foreach (var plugin in _plugins)
            {
                return plugin.PluginInstance.OnPacketSent(ep, packet);
            }
            return true;
        }

        public static void EntityAttack(Player player, Entity entity)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnEntityAttack(player, entity);
            }
        }

        public static void PlayerAttack(Player player, Player victim)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerAttack(player, victim);
            }
        }
    }

    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            return null;
        }
    }

    public class LoadedPlugin
    {
        public Plugin PluginInstance { get; set; }
        public PluginLoadContext LoadContext { get; set; }
        public string Path { get; set; }
    }
}

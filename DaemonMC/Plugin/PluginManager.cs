using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using DaemonMC.Entities;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Plugin.Events;
using DaemonMC.Utils.Text;

namespace DaemonMC.Plugin
{
    public class PluginManager
    {
        private static readonly List<LoadedPlugin> _plugins = new List<LoadedPlugin>();
        private static readonly Dictionary<string, DateTime> _reloadDelays = new Dictionary<string, DateTime>();
        private static readonly object _watcherLock = new();
        private static FileSystemWatcher? _watcher;

        public static void LoadPlugins(string pluginDirectory)
        {
            var lib = Path.Combine(pluginDirectory, "SharedLibraries");

            if (!Directory.Exists(pluginDirectory))
            {
                Log.warn($"{pluginDirectory}/ not found. Creating new folder...");
                Directory.CreateDirectory(pluginDirectory);
                Directory.CreateDirectory(lib);
            }

            foreach (var file in Directory.GetFiles(lib, "*.dll"))
            {
                LoadLibrary(file);
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

        public static void LoadLibrary(string filePath)
        {
            var fullPath = Path.GetFullPath(filePath);

            try
            {
                Assembly.LoadFrom(fullPath);

                Log.info($"Loading library: {filePath}");
            }
            catch (Exception ex)
            {
                Log.error($"Couldn't load library {filePath}: {ex.Message}");
            }
        }

        public static void LoadPlugin(string filePath, bool reloaded = false)
        {
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
                    var attr = type.GetCustomAttribute<PluginInfo>();
                    if (attr != null)
                    {
                        Log.info($"Loading plugin: {filePath} - {attr.Name} v{attr.Version}");
                    }
                    else
                    {
                        Log.error($"Plugin {filePath} can't be loaded. Missing metadata. Please check DaemonMC/wiki/Plugin-tutorial.");
                        return;
                    }

                    var pluginInstance = (Plugin)Activator.CreateInstance(type)!;

                    if (reloaded)
                    {
                        if (pluginInstance is not HotReload)
                        {
                            Log.error($"Plugin {filePath} can't be loaded. This plugin doesn't support HotReload feature. Please restart server to enable this plugin.");
                            return;
                        }
                        pluginInstance.OnReload();
                    }
                    else
                    {
                        pluginInstance.OnLoad();
                    }

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
            LoadPlugin(file, true);
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
            if (plugin.PluginInstance is not HotReload) return;

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
                Task.Delay(500).ContinueWith(_ => LoadPlugin(newPath, true));
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
                plugin.PluginInstance.OnPlayerJoined(new PlayerJoinedEvent(player));
            }
        }

        public static void PlayerLeaved(Player player)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerLeaved(new PlayerLeavedEvent(player));
            }
        }

        public static void PlayerMove(Player player, PlayerAuthInput movePk)
        {
            var ev = new PlayerMoveEvent(player);
            
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerMove(ev);
                if (ev.IsCancelled)
                {
                    break;
                }
            }

            if (ev.IsCancelled)
            {
                if (player.Position != movePk.Position)
                {
                    var packet = new MovePlayer
                    {
                        ActorRuntimeId = player.EntityID,
                        Position = player.Position,
                        Rotation = player.Rotation,
                        YheadRotation = player.HeadRotation,
                        Tick = player.Tick
                    };
                    player.Send(packet);
                }
            }
            else
            {
                player.Position = movePk.Position;
            }
        }

        public static bool PacketReceived(IPEndPoint ep, Packet packet)
        {
            var ev = new PacketEvent(ep, packet);

            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPacketReceived(ev);

                if (ev.IsCancelled)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool PacketSent(IPEndPoint ep, Packet packet)
        {
            var ev = new PacketEvent(ep, packet);

            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPacketSent(ev);

                if (ev.IsCancelled)
                {
                    return false;
                }
            }
            return true;
        }

        public static void PlayerAttackedEntity(Player player, Entity entity)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerAttackedEntity(new PlayerAttackedEntityEvent(player, entity));
            }
        }

        public static void PlayerAttackedPlayer(Player player, Player victim)
        {
            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerAttackedPlayer(new PlayerAttackedPlayerEvent(player, victim));
            }
        }

        public static void PlayerSentMessage(Player player, TextMessage textMessage)
        {
            var ev = new PlayerSentMessageEvent(player, textMessage);

            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerSentMessage(ev);
                if (ev.IsCancelled)
                {
                    break;
                }
            }

            if (!ev.IsCancelled)
            {
                var msg = new TextMessage
                {
                    MessageType = 1,
                    Username = player.Username,
                    Message = textMessage.Message
                };
                player.CurrentWorld.Send(msg);
            }
        }

        public static void PlayerSkinChanged(Player player, PlayerSkin playerSkin)
        {
            var ev = new PlayerSkinChangedEvent(player, playerSkin);

            foreach (var plugin in _plugins)
            {
                plugin.PluginInstance.OnPlayerSkinChanged(ev);

                if (ev.IsCancelled)
                {
                    break;
                }
            }

            if (!ev.IsCancelled)
            {
                var pk2 = new PlayerSkin
                {
                    UUID = player.UUID,
                    Skin = playerSkin.Skin,
                    Name = playerSkin.Name,
                    OldName = player.Skin.SkinId,
                    Trusted = playerSkin.Trusted,
                };
                player.CurrentWorld.Send(pk2);

                player.Skin = playerSkin.Skin;
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

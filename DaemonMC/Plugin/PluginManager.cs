using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using DaemonMC.Entities;
using DaemonMC.Network;
using DaemonMC.Network.Bedrock;
using DaemonMC.Plugin.Events;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Log = DaemonMC.Utils.Text.Log;

namespace DaemonMC.Plugin;

public abstract class PluginManager {
    
    private static readonly List<LoadedPlugin> Plugins = [];
    private static readonly ConcurrentDictionary<string, DateTime> LastFileEdit = new();
    private static readonly Timer CleanupTimer = new(CleanupFileEditCache, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    private static readonly FileSystemWatcher Watcher = new();

    public static void LoadPlugins(string pluginDirectory) {

        if (!Directory.Exists(pluginDirectory)) {
            Log.warn($"{pluginDirectory}/ not found. Creating new folder...");
            Directory.CreateDirectory(pluginDirectory);
            Directory.CreateDirectory(Path.Combine(pluginDirectory, "SharedLibraries"));
            return;
        }

        foreach (var dir in Directory.GetDirectories(pluginDirectory)) {
            
            if (dir.EndsWith("SharedLibraries")) {
                continue;
            }

            var files = Directory.GetFiles(Path.GetFullPath(dir), "*.dll");
            if (files.Length > 0) {
                LoadPlugin(files.First());
                continue;
            }
            
            var sourcePath = Path.Combine(Path.GetFullPath(dir), "Program.cs");
            if (!File.Exists(sourcePath)) {
                Log.warn($"Unable to load plugin {dir}! Program.cs or DLL not found.");
                continue;
            }
            
            LoadSourcePlugin(sourcePath);
        }

        if (DaemonMC.HotReloading) {
            EnableWatcher(pluginDirectory);
        }
    }
    
    private static void CleanupFileEditCache(object? state) {
        
        var cutoff = DateTime.Now.AddMinutes(-5);
        foreach (var kvp in LastFileEdit.Where(kvp => kvp.Value < cutoff).ToList()) {
            LastFileEdit.TryRemove(kvp.Key, out _);
        }
    }

    private static void OnFileRemoved(object source, FileSystemEventArgs e) {
        
        var extension = Path.GetExtension(e.FullPath).ToLower();
        if (extension != ".cs" && extension != ".dll") {
            return;
        }
        
        var pluginToRemove = Plugins.FirstOrDefault(p => p.Path.Equals(e.FullPath, StringComparison.OrdinalIgnoreCase));
        if (pluginToRemove != null) {
            UnloadPlugin(pluginToRemove);
            Plugins.Remove(pluginToRemove);
            return;
        }
        
        var affectedPlugin = FindPluginUsingDll(e.FullPath);
        if (affectedPlugin != null) {
            UnloadPlugin(affectedPlugin);
            Plugins.Remove(affectedPlugin);
            return;
        }

        Log.warn($"File removed but no associated plugin found: {e.FullPath}");
    }

    private static void OnFileChanged(object source, FileSystemEventArgs e) {
        
        var extension = Path.GetExtension(e.FullPath).ToLower();
        if (extension != ".cs" && extension != ".dll") {
            return;
        }

        if (LastFileEdit.ContainsKey(e.FullPath)) {
            if (DateTime.Now - LastFileEdit[e.FullPath] < TimeSpan.FromSeconds(1)) {
                return;
            }
        }
    
        LastFileEdit[e.FullPath] = DateTime.Now;
        if (IsFilePartOfPlugin(e.FullPath, out var plugin) && extension == ".cs") {
            if (plugin == null!) {
                LoadSourcePlugin(FindPluginProgramCs(e.FullPath)!);
                return;
            }
            
            ReloadPlugin(plugin);
        } else if (extension == ".dll") {
            
            var possiblePlugin = FindPluginUsingDll(e.FullPath);
            if (possiblePlugin != null) {
                Log.info($"Dependency edited for plugin '{possiblePlugin.PluginInstance.GetType().Name}': {e.FullPath}");
                ReloadPlugin(possiblePlugin);
            } else {
                LoadPlugin(e.FullPath);
            }
        }
    }
    
    public static string? FindPluginProgramCs(string filePath) {
        
        try {
            
            var currentDir = Path.GetDirectoryName(Path.GetFullPath(filePath));
            while (!string.IsNullOrEmpty(currentDir)) {
                
                var programPath = Path.Combine(currentDir, "Program.cs");
                if (File.Exists(programPath)) {
                    return programPath;
                }

                if (string.Equals(Path.GetFileName(currentDir), "Plugins", StringComparison.OrdinalIgnoreCase)) {
                    break;
                }
                
                currentDir = Directory.GetParent(currentDir)?.FullName;
            }
        } catch (Exception ex) {
            Log.error($"FindPluginProgramCs: {ex.Message}");
        }

        return null;
    }


    private static LoadedPlugin? FindPluginUsingDll(string dllPath) {
        
        foreach (var plugin in Plugins) {
            try {
                foreach (var assembly in plugin.LoadContext.Assemblies) {
                    if (assembly.Location.Equals(dllPath, StringComparison.OrdinalIgnoreCase)) {
                        return plugin;
                    }
                }
            } catch { // Ignore
            }
        }
        return null;
    }
    
    private static bool IsFilePartOfPlugin(string filePath, out LoadedPlugin? affectedPlugin) {
        
        affectedPlugin = null;
        
        filePath = Path.GetFullPath(filePath);
        var fileDir = Path.GetDirectoryName(filePath);
    
        if (string.IsNullOrEmpty(fileDir))
            return false;
        
        foreach (var plugin in Plugins) {
            
            var pluginDir = Path.GetDirectoryName(Path.GetFullPath(plugin.Path));
            if (string.IsNullOrEmpty(pluginDir) || (!fileDir.Equals(pluginDir, StringComparison.OrdinalIgnoreCase) && !fileDir.StartsWith(pluginDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))) {
                continue;
            }
            
            affectedPlugin = plugin;
            return true;
        }
        
        var currentDir = fileDir;
        while (currentDir != null) {
            
            var programCsPath = Path.Combine(currentDir, "Program.cs");
            if (File.Exists(programCsPath)) {
                return true;
            }

            if (Path.GetFileName(currentDir).Equals("Plugins", StringComparison.OrdinalIgnoreCase)) {
                break;
            }

            currentDir = Directory.GetParent(currentDir)?.FullName;
        }

        return false;
    }

    public static void UnloadPlugins() {
        
        Log.info("Unloading plugins...");
        foreach (var plugin in Plugins) {
            UnloadPlugin(plugin);
        }
        
        DisableWatcher();
        Plugins.Clear();
        
        Log.info("Plugins successfully unloaded.");
    }
    
    public static void UnloadPlugin(LoadedPlugin plugin) {
        
        try {

            var commands = CommandManager.GetCommandsByPlugin(plugin.PluginInstance);
            foreach (var command in commands) {
                CommandManager.Unregister(command.Name);
            }
            
            plugin.PluginInstance.OnUnload();
            if (plugin.PluginInstance.Resources is IDisposable disposable) {
                disposable.Dispose();
            }
            
            plugin.PluginInstance = null!;
            plugin.LoadContext.Unload();
            
            for (var i = 0; i < 5; i++) {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);
                GC.WaitForPendingFinalizers();
            }
            
            Log.info($"Unloaded plugin: {Path.GetFileName(plugin.Path)}");
            UpdateCommandsForAllPlayers();
        } catch (Exception ex) {
            Log.error($"Error unloading plugin {plugin.Path}: {ex}");
        }
    }

    private static void UpdateCommandsForAllPlayers() {

        foreach (var onlinePlayer in Server.GetOnlinePlayers()) {
            onlinePlayer.Send(new AvailableCommands {
                EnumValues = CommandManager.EnumValues,
                Enums = CommandManager.RealEnums,
                Commands = CommandManager.GetAvailableCommands()
            });
        }
    }
    
    private static void ReloadPlugin(LoadedPlugin plugin) {
        
        Plugins.Remove(plugin);
        UnloadPlugin(plugin);
        
        if (plugin.SourcePlugin) {
            LoadSourcePlugin(plugin.Path);
            return;
        }
        
        LoadPlugin(plugin.Path);
    }
    
    public static void LoadPlugin(string sourcePath) {
        
        try {
            
            var pluginDirectory = Path.GetDirectoryName(sourcePath)!;
            var sharedLibsPath = Path.Combine(Directory.GetParent(pluginDirectory)!.FullName, "SharedLibraries");
            var globalSharedLibsPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "SharedLibraries");
            
            byte[] assemblyBytes = [];
            var retryCount = 0;
            
            while (retryCount < 3) {
                
                try {
                    using var fs = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    assemblyBytes = new byte[fs.Length];
                    fs.ReadExactly(assemblyBytes, 0, assemblyBytes.Length);
                    break;
                } catch (IOException) when (retryCount < 2) {
                    retryCount++;
                    Thread.Sleep(100);
                }
            }

            if (assemblyBytes.Length == 0) {
                Log.error($"Failed to read plugin assembly from {sourcePath}");
                return;
            }

            var loadContext = new PluginLoadContext(sourcePath);
            var assembly = loadContext.LoadFromStream(new MemoryStream(assemblyBytes));
            
            void LoadDependency(string dllPath) {
                
                try {
                    
                    byte[] bytes;
                    using (var fs = new FileStream(dllPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                        bytes = new byte[fs.Length];
                        fs.ReadExactly(bytes, 0, bytes.Length);
                    }
                    
                    loadContext.LoadFromStream(new MemoryStream(bytes));
                    Log.debug($"Loaded shared library: {Path.GetFileName(dllPath)}");
                } catch (Exception ex) {
                    Log.error($"Failed to load shared library {dllPath}: {ex.Message}");
                }
            }

            if (Directory.Exists(sharedLibsPath)) {
                
                foreach (var dll in Directory.GetFiles(sharedLibsPath, "*.dll")) {
                    LoadDependency(dll);
                }
            }
            
            if (Directory.Exists(globalSharedLibsPath)) {
                
                foreach (var dll in Directory.GetFiles(globalSharedLibsPath, "*.dll")) {
                    LoadDependency(dll);
                }
            }

            var pluginLoaded = false;
            foreach (var type in assembly.GetTypes()) {
                
                if (!typeof(Plugin).IsAssignableFrom(type) || type is { IsInterface: true, IsAbstract: true }) {
                    continue;
                }

                try {
                    
                    var pluginInstance = (Plugin)Activator.CreateInstance(type)!;
                    pluginInstance.Resources = new PluginResources(Path.GetDirectoryName(sourcePath)!);
                    
                    pluginInstance.Resources.CreateResourcesFolder();
                    pluginInstance.OnLoad();

                    Plugins.Add(new LoadedPlugin {
                        PluginInstance = pluginInstance,
                        Path = sourcePath,
                        SourcePlugin = false,
                        LoadContext = loadContext
                    });
                    
                    pluginLoaded = true;
                    Log.info($"Loaded plugin: {type.Name} from {Path.GetFileName(sourcePath)}");
                    
                    UpdateCommandsForAllPlayers();
                } catch (Exception ex) {
                    Log.error($"Failed to instantiate plugin type {type.Name} from {sourcePath}: {ex.Message}");
                }
            }

            if (pluginLoaded) {
                return;
            }
            
            Log.warn($"No valid plugin class found in: {sourcePath}");
            loadContext.Unload();
        } catch (Exception ex) {
            Log.error($"Error loading plugin from DLL {sourcePath}: {ex}");
        }
    }
    
    public static void LoadSourcePlugin(string sourcePath) {
        
        try {
            
            var pluginDir = Path.GetDirectoryName(sourcePath)!;
            var allCsFiles = Directory.GetFiles(pluginDir, "*.cs");
            var syntaxTrees = new List<SyntaxTree>();
            
            foreach (var file in allCsFiles) {
                try {
                    var code = File.ReadAllText(file);
                    syntaxTrees.Add(CSharpSyntaxTree.ParseText(code));
                    Log.debug($"Parsed source file: {Path.GetFileName(file)}");
                } catch (Exception ex) {
                    Log.error($"Error parsing {file}: {ex.Message}");
                }
            }

            if (syntaxTrees.Count == 0) {
                Log.error($"No valid C# files found in {pluginDir}");
                return;
            }

            var assemblyName = Path.GetRandomFileName();
            var references = new List<MetadataReference> {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
            };

            references.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location)).Select(a => MetadataReference.CreateFromFile(a.Location)));
            references.Add(MetadataReference.CreateFromFile(typeof(Plugin).Assembly.Location));
            
            var globalSharedLibsPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "SharedLibraries");
            if (Directory.Exists(globalSharedLibsPath)) { 
                references.AddRange(Directory.GetFiles(globalSharedLibsPath, "*.dll").Select(dll => MetadataReference.CreateFromFile(dll)));
            }

            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release, concurrentBuild: true, metadataImportOptions: MetadataImportOptions.Public);
            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, references, compilationOptions);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            
            if (!result.Success) {
                
                Log.error($"Failed to compile plugin from {sourcePath}");
                foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)) {
                    Log.error(diagnostic.ToString());
                }
                
                return;
            }

            ms.Seek(0, SeekOrigin.Begin);
            
            var loadContext = new PluginLoadContext(sourcePath);
            var assembly = loadContext.LoadFromStream(ms);

            foreach (var type in assembly.GetTypes()) {

                if (!typeof(Plugin).IsAssignableFrom(type) || type is { IsInterface: true, IsAbstract: true }) { 
                    continue;
                }
                
                try {
                    
                    var pluginInstance = (Plugin)Activator.CreateInstance(type)!;
                    pluginInstance.Resources = new PluginResources(Path.GetDirectoryName(sourcePath)!);
                    
                    pluginInstance.Resources.CreateResourcesFolder();
                    pluginInstance.OnLoad();

                    Plugins.Add(new LoadedPlugin {
                        PluginInstance = pluginInstance,
                        Path = sourcePath,
                        SourcePlugin = true,
                        LoadContext = loadContext
                    });

                    Log.info($"Loaded plugin: {type.Name} from {Path.GetFileName(sourcePath)}");
                    UpdateCommandsForAllPlayers();
                } catch (Exception ex) {
                    Log.error($"Failed to instantiate plugin {type.Name}: {ex.Message}");
                }
            }
        }  catch (Exception ex) {
            Log.error($"Error loading source plugin from {sourcePath}: {ex}");
        }
    }

    private static void EnableWatcher(string dir) {
        
        Watcher.Path = Path.GetFullPath(dir);
        Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
        
        Watcher.IncludeSubdirectories = true;
        Watcher.Filter = "*.*";
        
        Watcher.Changed += OnFileChanged;
        Watcher.Renamed += OnFileChanged;
        
        Watcher.Created += OnFileChanged;
        Watcher.Deleted += OnFileRemoved;
        
        Watcher.EnableRaisingEvents = true;
    }

    private static void DisableWatcher() {
        
        Watcher.EnableRaisingEvents = false;
        Watcher.Changed -= OnFileChanged;
        Watcher.Renamed -= OnFileChanged;
        Watcher.Created -= OnFileChanged;
        Watcher.Deleted -= OnFileRemoved;
        Watcher.Dispose();
    }
    
    public static bool PlayerJoined(Player player) {
        
        var ev = new PlayerJoinedEvent(player);
        var playerKicked = false;
        
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerJoined(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            if (playerKicked) {
                continue;
            }
                
            player.Kick("Disconnected");
            playerKicked = true;
        }

        if (ev.IsCancelled) {
            return false;
        }

        if (!ev.IsJoinMessageEnabled()) {
            return true;
        }
        
        foreach (var onlinePlayer in Server.GetOnlinePlayers()) {
            onlinePlayer.SendMessage(ev.GetJoinMessage());
        }
        
        return true;
    }

    public static void PlayerLeaved(Player player) {
        
        var ev = new PlayerLeavedEvent(player);
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerLeaved(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            Log.warn("You can't cancel a PlayerLeavedEvent!");
        }
        
        foreach (var onlinePlayer in Server.GetOnlinePlayers()) {
            onlinePlayer.SendMessage(ev.GetLeaveMessage());
        }
    }

    public static bool PlayerMove(Player player) {

        var oldPosition = player.Position;
        var ev = new PlayerMoveEvent(player);
        
        var playerMoved = false;
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerMove(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            if (playerMoved) {
                continue;
            }
                
            player.MoveTo(oldPosition);
            playerMoved = true;
        }

        return !playerMoved;
    }
    
    public static bool PlayerAttackedEntity(Player player, Entity entity) {
        
        var ev = new PlayerAttackedEntityEvent(player, entity);
        var damageNotSent = false;
        
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerAttackedEntity(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            damageNotSent = true;
        }

        return !damageNotSent;
    }

    public static bool PlayerAttackedPlayer(Player attacker, Player victim) {
        
        var ev = new PlayerAttackedPlayerEvent(attacker, victim);
        var damageNotSent = false;
        
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerAttackedPlayer(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            damageNotSent = true;
        }

        return !damageNotSent;
    }
    
    public static bool PlayerSentMessage(Player player, TextMessage textMessage) {
        
        var ev = new PlayerSentMessageEvent(player, textMessage);
        var messageSent = false;
        
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerSentMessage(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            messageSent = true;
        }

        return !messageSent;
    }
    
    public static bool PlayerSkinChanged(Player player, PlayerSkin playerSkin) {
        
        var ev = new PlayerSkinChangedEvent(player, playerSkin);
        var skinChanged = false;
        
        foreach (var plugin in Plugins) {
            
            plugin.PluginInstance.OnPlayerSkinChanged(ev);
            if (!ev.IsCancelled) {
                continue;
            }
            
            skinChanged = true;
        }

        return !skinChanged;
    }
    
    public static bool PacketReceived(IPEndPoint ep, Packet packet) {
        
        foreach (var plugin in Plugins) {
            return plugin.PluginInstance.OnPacketReceived(ep, packet);
        }
        
        return true;
    }

    public static bool PacketSent(IPEndPoint ep, Packet packet) {
        
        foreach (var plugin in Plugins) {
            return plugin.PluginInstance.OnPacketSent(ep, packet);
        }
        
        return true;
    }

    public static List<LoadedPlugin> GetLoadedPlugins() {
        return Plugins;
    }
}

public class LoadedPlugin {

    public required Plugin PluginInstance { get; set; }
    public required string Path { get; set; }
    public required bool SourcePlugin { get; set; }
    public required PluginLoadContext LoadContext { get; set; }
}

public class PluginLoadContext : AssemblyLoadContext {
    
    private readonly AssemblyDependencyResolver _resolver;
    private readonly string _sharedLibrariesPath;
    private readonly string _globalSharedLibrariesPath;

    public PluginLoadContext(string pluginPath) : base(isCollectible: true) {
        
        _resolver = new AssemblyDependencyResolver(pluginPath);
        
        var pluginDir = Path.GetDirectoryName(pluginPath)!;
        _sharedLibrariesPath = Path.Combine(Directory.GetParent(pluginDir)!.FullName, "SharedLibraries");
        _globalSharedLibrariesPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins", "SharedLibraries");
    }

    protected override Assembly? Load(AssemblyName assemblyName) {

        var path = _resolver.ResolveAssemblyToPath(assemblyName);
        if (path != null) {
            return LoadFromAssemblyPath(path);
        }
        
        var localLib = Path.Combine(_sharedLibrariesPath, $"{assemblyName.Name}.dll");
        if (File.Exists(localLib)) {
            return LoadFromAssemblyPath(localLib);
        }
        
        var globalLib = Path.Combine(_globalSharedLibrariesPath, $"{assemblyName.Name}.dll");
        return File.Exists(globalLib) ? LoadFromAssemblyPath(globalLib) : null;
    }
    
    public void UnloadPlugin() {
        Unload();
    }
}

namespace DaemonMC.Plugin;

public class PluginResources(string path) {

    public void CreateResourcesFolder() {
        
        var resourcesPath = Path.Combine(path, "Resources");
        if (!Directory.Exists(resourcesPath)) {
            Directory.CreateDirectory(resourcesPath);
        }
    }

    public string GetResourcesPath() {
        return Path.Combine(path, "Resources");
    }
}
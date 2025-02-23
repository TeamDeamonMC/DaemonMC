using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC
{
    public class ResourcePackManager
    {
        public static Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
        public static int ChunkSize = 10000;
        public static bool ForcePacks = false;
        public static void LoadPacks(string packdDirectory)
        {
            if (!Directory.Exists(packdDirectory))
            {
                Log.warn($"{packdDirectory}/ not found. Creating new folder...");
            }
            Directory.CreateDirectory(packdDirectory);

            foreach (string file in Directory.GetFiles(packdDirectory))
            {
                if (Path.GetExtension(file) == ".mcpack")
                {
                    Server.packs.Add(new ResourcePack(file));
                }
            }
        }

        public static byte[] GetData(string contentId, int chunkIndex)
        {
            ResourcePack resourcePack = Server.packs.FirstOrDefault(p => p.ContentId == contentId);

            int start = chunkIndex * ChunkSize;
            int length = Math.Min(ChunkSize, resourcePack.PackContent.Length - start);
            byte[] chunk = new byte[length];
            Array.Copy(resourcePack.PackContent, start, chunk, 0, length);
            return chunk;
        }

        public static void RegisterAnimation(string animationID, string controllerName, string animationName, string nextAnimationName = "")
        {
            Animations.Add(animationID, new Animation(controllerName, animationName, nextAnimationName));
        }
    }

    public class resourceManifest
    {
        public int format_version { get; set; } = 0;
        public Header header { get; set; } = new Header();
        public List<Modules> modules { get; set; } = new List<Modules>();
    }

    public class Header
    {
        public string description { get; set; } = "";
        public List<int> min_engine_version { get; set; } = new List<int>();
        public string name { get; set; } = "";
        public string uuid { get; set; } = "";
        public List<int> version { get; set; } = new List<int>();
    }

    public class Modules
    {
        public string type { get; set; } = "";
        public string uuid { get; set; } = "";
        public List<int> version { get; set; } = new List<int>();
    }
}

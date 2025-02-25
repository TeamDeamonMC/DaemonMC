using System.IO.Compression;
using DaemonMC.Utils.Text;
using Newtonsoft.Json;

namespace DaemonMC
{
    public class ResourcePack
    {
        public Guid UUID { get; set; } = new Guid();
        public string PackIdVersion { get; set; } = "";
        public byte[] PackContent { get; set; } = new byte[0];
        public string ContentKey { get; set; } = "";
        public string SubpackName { get; set; } = "";
        public string ContentId { get; set; } = "";
        public bool HasScripts { get; set; } = false;
        public bool IsAddon { get; set; } = false;
        public bool RayTracking { get; set; } = false;
        public string CdnUrl { get; set; } = "";
        public ResourcePack(string PackPath)
        {
            var pack = ZipFile.OpenRead(PackPath);

            string keyFile = Path.Combine("Resource Packs", $"{Path.GetFileNameWithoutExtension(PackPath)}.key");

            string manifest = "";

            for (byte i = 0; i < pack.Entries.Count; i++)
            {
                if (pack.Entries[i].ToString() == "manifest.json")
                {
                    manifest = pack.Entries[i].ToString();
                }
            }

            if (manifest == "")
            {
                Log.error($"Couldn't load resource pack: Resource Packs\\{Path.GetFileNameWithoutExtension(PackPath)}.mcpack. No manifest.json found)");
            }
            else
            {
                PackContent = File.ReadAllBytes(PackPath);

                using (var stream = pack.GetEntry(manifest).Open())
                using (var reader = new StreamReader(stream))
                {
                    resourceManifest? content = JsonConvert.DeserializeObject<resourceManifest>(reader.ReadToEnd());

                    UUID = new Guid(content.header.uuid);
                    PackIdVersion = $"{content.header.version[0]}.{content.header.version[1]}.{content.header.version[2]}";
                    if (File.Exists(keyFile))
                    {
                        ContentKey = File.ReadAllText(keyFile);
                    }
                    ContentId = content.header.uuid;
                }
                Log.info($"Loading resource pack: Resource Packs\\{Path.GetFileNameWithoutExtension(PackPath)}.mcpack (size:{PackContent.Count()} encryption:{!string.IsNullOrEmpty(ContentKey)})");
                Log.debug($"Loaded pack: {Path.GetFileNameWithoutExtension(PackPath)} UUID:{UUID} PackIdVersion:{PackIdVersion} PackSize:{PackContent.Count()}");
            }
        }
    }
}

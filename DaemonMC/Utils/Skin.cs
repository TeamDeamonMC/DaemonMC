using Newtonsoft.Json;

namespace DaemonMC.Utils
{
    public class Skin
    {
        public string ArmSize { get; set; } = "";
        public List<AnimatedImageData> AnimatedImageData { get; set; } = new List<AnimatedImageData>();
        public bool OverrideSkin { get; set; }
        public List<PersonaPiece> PersonaPieces { get; set; } = new List<PersonaPiece>();
        public bool PersonaSkin { get; set; }
        public List<PieceTintColor> PieceTintColors { get; set; } = new List<PieceTintColor>();
        public string PlayFabId { get; set; } = "";
        public bool PremiumSkin { get; set; }
        public string SkinAnimationData { get; set; } = "";
        public string SkinColor { get; set; } = "";
        public byte[] SkinData { get; set; } = new byte[0];
        public string SkinGeometryData { get; set; } = "";
        public string SkinGeometryDataEngineVersion { get; set; } = "0.0.0";
        public string SkinId { get; set; } = "";
        public int SkinImageHeight { get; set; }
        public int SkinImageWidth { get; set; }
        public string SkinResourcePatch { get; set; } = "";
        public Cape Cape { get; set; } = new Cape();
        public bool CapeOnClassicSkin { get; set; }

        public static Skin Create(string skinPng, string geometryJson)
        {
            var json = File.ReadAllText(geometryJson);
            var geometry = JsonConvert.DeserializeObject<GeometryRoot>(json);
            var desription = geometry.minecraftgeometry.FirstOrDefault(g => g.Description.Identifier == "geometry.humanoid.custom").Description;

            return new Skin()
            {
                ArmSize = "wide",
                PremiumSkin = true,
                SkinData = Texture.PngToBytes(skinPng),
                SkinGeometryData = json,
                SkinImageHeight = (int)desription.Texture_Height,
                SkinImageWidth = (int)desription.TextureWidth,
                SkinResourcePatch = "{\n   \"geometry\" : {\n      \"default\" : \"geometry.humanoid.custom\"\n   }\n}\n",
                CapeOnClassicSkin = false,
            };
        }
    }

    public class GeometryRoot
    {
        [JsonProperty("format_version")]
        public string Version { get; set; }
        [JsonProperty("minecraft:geometry")]
        public List<MinecraftGeometry> minecraftgeometry { get; set; }
    }

    public class MinecraftGeometry
    {
        [JsonProperty("description")]
        public Description Description { get; set; }
    }

    public class Description
    {
        [JsonProperty("identifier")]
        public string Identifier { get; set; }
        [JsonProperty("texture_width")]
        public float TextureWidth { get; set; }
        [JsonProperty("texture_height")]
        public float Texture_Height { get; set; }
        [JsonProperty("visible_bounds_width")]
        public float VisibleBoundsWidth { get; set; }
        [JsonProperty("visible_bounds_height")]
        public double VisibleBoundsHeight { get; set; }
        [JsonProperty("visible_bounds_offset")]
        public List<double> VisibleBoundsOffset { get; set; }
    }

    public class Cape
    {
        public byte[] CapeData { get; set; } = new byte[0];
        public string CapeId { get; set; } = "";
        public int CapeImageHeight { get; set; }
        public int CapeImageWidth { get; set; }
        public bool CapeOnClassicSkin { get; set; }
    }

    public class AnimatedImageData
    {
        public int AnimationExpression { get; set; }
        public float Frames { get; set; }
        public string Image { get; set; } = "";
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public int Type { get; set; }
    }

    public class PersonaPiece
    {
        public string PieceId { get; set; } = "";
        public string PieceType { get; set; } = "";
        public string PackId { get; set; } = "";
        public bool IsDefault { get; set; }
        public string ProductId { get; set; } = "";
    }

    public class PieceTintColor
    {
        public List<string> Colors { get; set; } = new();
        public string PieceType { get; set; } = "";
    }
}
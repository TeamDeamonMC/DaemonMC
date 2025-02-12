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
        public string SkinGeometryDataEngineVersion { get; set; } = "";
        public string SkinId { get; set; } = "";
        public int SkinImageHeight { get; set; }
        public int SkinImageWidth { get; set; }
        public string SkinResourcePatch { get; set; } = "";
        public Cape Cape { get; set; } = new Cape();
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
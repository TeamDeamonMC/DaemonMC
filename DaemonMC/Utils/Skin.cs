namespace DaemonMC.Utils
{
    public class Skin
    {
        public string ArmSize { get; set; }
        public bool OverrideSkin { get; set; }
        public List<object> PersonaPieces { get; set; }
        public bool PersonaSkin { get; set; }
        public List<object> PieceTintColors { get; set; }
        public string PlayFabId { get; set; }
        public bool PremiumSkin { get; set; }
        public string SkinAnimationData { get; set; }
        public string SkinColor { get; set; }
        public string SkinData { get; set; }
        public string SkinGeometryData { get; set; }
        public string SkinGeometryDataEngineVersion { get; set; }
        public string SkinId { get; set; }
        public int SkinImageHeight { get; set; }
        public int SkinImageWidth { get; set; }
        public string SkinResourcePatch { get; set; }
        public Cape Cape { get; set; }
    }

    public class Cape
    {
        public string CapeData { get; set; }
        public string CapeId { get; set; }
        public int CapeImageHeight { get; set; }
        public int CapeImageWidth { get; set; }
        public bool CapeOnClassicSkin { get; set; }
    }
}

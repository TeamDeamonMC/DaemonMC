namespace DaemonMC.Utils
{
    public class JwtPayload
    {
        public string ArmSize { get; set; } = "";
        public List<AnimatedImageData> AnimatedImageData { get; set; } = new List<AnimatedImageData>();
        public string CapeData { get; set; } = "";
        public string CapeId { get; set; } = "";
        public int CapeImageHeight { get; set; }
        public int CapeImageWidth { get; set; }
        public bool CapeOnClassicSkin { get; set; }
        public long ClientRandomId { get; set; }
        public bool CompatibleWithClientSideChunkGen { get; set; }
        public int CurrentInputMode { get; set; }
        public int DefaultInputMode { get; set; }
        public string DeviceId { get; set; } = "";
        public string DeviceModel { get; set; } = "";
        public int DeviceOS { get; set; }
        public string GameVersion { get; set; } = "";
        public int GuiScale { get; set; }
        public bool IsEditorMode { get; set; }
        public string LanguageCode { get; set; } = "";
        public int MaxViewDistance { get; set; }
        public int MemoryTier { get; set; }
        public bool OverrideSkin { get; set; }
        public List<PersonaPiece> PersonaPieces { get; set; } = new List<PersonaPiece>();
        public bool PersonaSkin { get; set; }
        public List<PieceTintColor> PieceTintColors { get; set; } = new List<PieceTintColor>();
        public string PlatformOfflineId { get; set; } = "";
        public string PlatformOnlineId { get; set; } = "";
        public int PlatformType { get; set; }
        public string PlayFabId { get; set; } = "";
        public bool PremiumSkin { get; set; }
        public string SelfSignedId { get; set; } = "";
        public string ServerAddress { get; set; } = "";
        public string SkinAnimationData { get; set; } = "";
        public string SkinColor { get; set; } = "";
        public string SkinData { get; set; } = "";
        public string SkinGeometryData { get; set; } = "";
        public string SkinGeometryDataEngineVersion { get; set; } = "";
        public string SkinId { get; set; } = "";
        public int SkinImageHeight { get; set; }
        public int SkinImageWidth { get; set; }
        public string SkinResourcePatch { get; set; } = "";
        public string ThirdPartyName { get; set; } = "";
        public bool ThirdPartyNameOnly { get; set; }
        public bool TrustedSkin { get; set; }
        public int UIProfile { get; set; }
    }
}

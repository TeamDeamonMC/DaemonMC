namespace DaemonMC.Network.Enumerations
{
    public enum PlayStatusTypes
    {
        LoginSuccess,
        LoginFailed_ClientOld,
        LoginFailed_ServerOld,
        PlayerSpawn,
        LoginFailed_InvalidTenant,
        LoginFailed_EditionMismatchEduToVanilla,
        LoginFailed_EditionMismatchVanillaToEdu,
        LoginFailed_ServerFullSubClient,
        LoginFailed_EditorMismatchEditorToVanilla,
        LoginFailed_EditorMismatchVanillaToEditor,
    }
}

namespace DaemonMC.Plugin.Plugin
{
    public interface Plugin
    {
        void OnLoad();
        void OnUnload();

        void OnPlayerJoin(Player player);
    }
}

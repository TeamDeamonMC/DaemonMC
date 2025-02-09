namespace DaemonMC.Plugin.Plugin
{
    public interface IPlugin
    {
        void OnLoad();
        void OnUnload();
    }

    public interface IPlayerPlugin : IPlugin
    {
        void OnPlayerJoin(Player player);
    }
}

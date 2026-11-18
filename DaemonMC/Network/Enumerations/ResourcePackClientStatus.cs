namespace DaemonMC.Network.Enumerations
{
    public enum ResourcePackClientResponseStatus
    {
        Refused = 1,
        SendPacks,
        HaveAllPacks,
        Completed,
    }

    public class ResourcePackClientResponseStatusNames
    {
        public static string[] Values = ["cancel", "downloading", "downloadingfinished", "resourcepackstackfinished"];
    }
}

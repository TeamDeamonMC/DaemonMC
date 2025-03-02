using Newtonsoft.Json.Serialization;

namespace DaemonMC.Forms
{
    public static class FormManager
    {
        public static Dictionary<long, (Form form, Action<Player, string> callback)> PendingForms = new Dictionary<long, (Form form, Action<Player, string> callback)>();
        public static DefaultContractResolver contractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() };
    }
}

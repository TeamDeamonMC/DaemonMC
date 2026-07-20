using DaemonMC.Utils.Game;
using DaemonMC.Utils.Text;

namespace DaemonMC.Items
{
    public class CreativeContentManager
    {
        public static List<CreativeItemGroup> Groups { get; set; } = new();
        private static readonly List<Item> _creativeItems = new();

        public static void AddGroup(CreativeItemGroup itemGroup)
        {
            CreativeItemGroup? group = Groups.FirstOrDefault(g => g.Name == itemGroup.Name);
            if (group == null || itemGroup.Name == "")
            {
                Groups.Add(itemGroup);
            }
            else
            {
                Log.warn($"[CreativeContentManager] Couldn't add {itemGroup.Name}. Group already exist at index {GetGroupIndex(itemGroup.Name)}.");
            }
        }

        public static void RemoveGroup(string groupName)
        {
            CreativeItemGroup? group = GetGroup(groupName);
            if (group == null)
            {
                Log.warn($"[CreativeContentManager] Couldn't remove group with name {groupName}. Group doesn't exist.");
            }
            else
            {
                Groups.Remove(group);
            }
        }

        public static CreativeItemGroup? GetGroup(string groupName)
        {
            CreativeItemGroup? group = Groups.FirstOrDefault(g => g.Name == groupName);
            if (group == null)
            {
                Log.warn($"[CreativeContentManager] Group name {groupName} doesn't exist. Returned null.");
            }
            return group;
        }

        public static int GetGroupIndex(string groupName)
        {
            CreativeItemGroup? group = GetGroup(groupName);
            if (group == null)
            {
                Log.warn($"[CreativeContentManager] Group name {groupName} doesn't exist for GetGroupIndex(string). Returned 0");
                return 0;
            }
            else
            {
                return Groups.IndexOf(group);
            }
        }

        public static Item? GetItemByNetId(int creativeNetId)
        {
            if ((creativeNetId - 1) >= _creativeItems.Count)
            {
                Log.warn($"[CreaftCreativeAction] Couldn't get item by {creativeNetId}. CreativeContentManager was probably modified during runtime. More info: github.com/TeamDeamonMC/DaemonMC/wiki/API-%E2%80%90-CreativeContentManager");
                return null;
            }
            return _creativeItems[creativeNetId - 1];
        }

        public static void Init()
        {
            _creativeItems.Clear();

            foreach (var group in Groups)
            {
                foreach (var item in group.Items)
                {
                    _creativeItems.Add(item);
                }
            }

            Log.debug($"[CreativeContentManager] Initialized {_creativeItems.Count} items");
        }
    }
}

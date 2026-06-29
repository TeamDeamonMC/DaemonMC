using DaemonMC.Items;
using DaemonMC.Network.Enumerations;

namespace DaemonMC.Utils.Game;

public class CreativeItemGroup
{
    public int GroupId { get; set; }

    public CreativeCategoryType Category { get; set; }

    public string Name { get; set; } = string.Empty;

    public Item? Icon { get; set; }
}
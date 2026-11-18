namespace DaemonMC.Network.Enumerations
{
    public enum ItemStackRequestActionType
    {
        Take,
        Place,
        Swap,
        Drop,
        Destroy,
        Consume,
        Create,
        PlaceInItemContainer_DEPRECATED,
        TakeFromItemContainer_DEPRECATED,
        ScreenLabTableCombine,
        ScreenBeaconPayment,
        ScreenHUDMineBlock,
        CraftRecipe,
        CraftRecipeAuto,
        CraftCreative,
        CraftRecipeOptional,
        CraftRepairAndDisenchant,
        CraftLoom,
        CraftNonImplemented_DEPRECATEDASKTYLAING,
        CraftResults_DEPRECATEDASKTYLAING,
        ifdef,
        TEST_INFRASTRUCTURE_ENABLED,
        Test,
        endif,
        CraftReserved
    }

    public static class ItemStackRequestActionTypeMapper
    {
        private static readonly ItemStackRequestActionType[] v1_26_40 =
        [
            ItemStackRequestActionType.Take,
            ItemStackRequestActionType.Place,
            ItemStackRequestActionType.Swap,
            ItemStackRequestActionType.Drop,
            ItemStackRequestActionType.Destroy,
            ItemStackRequestActionType.Consume,
            ItemStackRequestActionType.Create,
            ItemStackRequestActionType.ScreenLabTableCombine,
            ItemStackRequestActionType.ScreenBeaconPayment,
            ItemStackRequestActionType.ScreenHUDMineBlock,
            ItemStackRequestActionType.CraftRecipe,
            ItemStackRequestActionType.CraftRecipeAuto,
            ItemStackRequestActionType.CraftCreative,
            ItemStackRequestActionType.CraftRecipeOptional,
            ItemStackRequestActionType.CraftRepairAndDisenchant,
            ItemStackRequestActionType.CraftLoom,
            ItemStackRequestActionType.CraftNonImplemented_DEPRECATEDASKTYLAING,
            ItemStackRequestActionType.CraftResults_DEPRECATEDASKTYLAING,
        ];

        private static readonly ItemStackRequestActionType[] v1_26_60 =
        [
            ItemStackRequestActionType.Take,
            ItemStackRequestActionType.Place,
            ItemStackRequestActionType.Swap,
            ItemStackRequestActionType.Drop,
            ItemStackRequestActionType.Destroy,
            ItemStackRequestActionType.Consume,
            ItemStackRequestActionType.Create,
            ItemStackRequestActionType.ScreenLabTableCombine,
            ItemStackRequestActionType.ScreenBeaconPayment,
            ItemStackRequestActionType.ScreenHUDMineBlock,
            ItemStackRequestActionType.CraftRecipe,
            ItemStackRequestActionType.CraftRecipeAuto,
            ItemStackRequestActionType.CraftCreative,
            ItemStackRequestActionType.CraftRecipeOptional,
            ItemStackRequestActionType.CraftRepairAndDisenchant,
            ItemStackRequestActionType.CraftLoom,
            ItemStackRequestActionType.CraftReserved,
            ItemStackRequestActionType.CraftNonImplemented_DEPRECATEDASKTYLAING,
            ItemStackRequestActionType.CraftResults_DEPRECATEDASKTYLAING,
        ];

        public static ItemStackRequestActionType FromNetworkId(int id, int protocolVersion)
        {
            ItemStackRequestActionType[]? map = null;
            if (protocolVersion >= Info.v1_26_60)
            {
                map = v1_26_60;
            }
            else if (protocolVersion >= Info.v1_26_40)
            {
                map = v1_26_40;
            }

            if (map == null)
            {
                return (ItemStackRequestActionType)id;
            }

            if (id < 0 || id >= map.Length)
            {
                throw new InvalidDataException($"Unknown item stack request action type {id} for protocol {protocolVersion}");
            }
            return map[id];
        }
    }
}

using BattleTech;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("OnDayPassed")]
    public static class SimGameState_OnDayPassed
    {
        public static FactionValue owner;
        [HarmonyPostfix]
        public static void RefreshShop()
        {
            Control.LogDebug(DInfo.RefreshShop, $"Refreshing Shops Daily {Control.State.CurrentSystem.Def.Description.Name}");
            Control.RefreshShops("Daily");
            if (owner != Control.State.CurrentSystem.OwnerValue)
            {
                if (owner != null)
                {
                    Control.LogDebug(DInfo.RefreshShop, $"Refreshing Shops OwnerChange {Control.State.CurrentSystem.Def.Description.Name}");
                    Control.RefreshShops("OwnerChange");
                }
                owner = Control.State.CurrentSystem.OwnerValue;
            }

        }
    }

    [HarmonyPatch(typeof(StarSystem))]
    [HarmonyPatch("RefreshShops")]
    public static class StarSystem_RefreshShops
    {
        [HarmonyPrefix]
        public static bool RefreshShops()
        {
            SimGameState_OnDayPassed.owner = Control.State.CurrentSystem.OwnerValue;

            Control.LogDebug(DInfo.RefreshShop, $"Refreshing Shops SystemChange {Control.State.CurrentSystem.Def.Description.Name}");
            Control.RefreshShops("SystemChange");

            return false;
        }
    }

    [HarmonyPatch(typeof(StarSystem))]
    [HarmonyPatch("CompletedContract")]
    public static class StarSystem_CompletedContract
    {
        [HarmonyPostfix]
        public static void RefreshShops()
        {
            Control.LogDebug(DInfo.RefreshShop, $"Refreshing Shops ContractComplete {Control.State.CurrentSystem.Def.Description.Name}");
            Control.RefreshShops("ContractComplete");
        }
    }

    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("DeductQuarterlyFunds")]
    public static class SimGameState_DeductQuarterlyFunds
    {
        [HarmonyPostfix]
        public static void RefreshShops()
        {
            Control.LogDebug(DInfo.RefreshShop, $"Refreshing Shops MonthEnd {Control.State.CurrentSystem.Def.Description.Name}");
            Control.RefreshShops("MonthEnd");
        }
    }
}
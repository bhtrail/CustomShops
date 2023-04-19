using System.Linq;
using BattleTech;
using BattleTech.UI;

namespace CustomShops.Patches
{
    public static class SGNavigationButton_ManageShopFlyout
    {
        [HarmonyPrefix]
        public static bool ShowButton(SGNavigationButton __instance)
        {
            var active = Control.Shops.Any(i => i.Exists && i.CanUse);
            if (active)
            {
                __instance.AddFlyoutButton("Store", DropshipMenuType.Shop);
            }

            return false;
        }
    }

    public static class IsShopActive
    {
        public static bool ShowButton(ref bool __result)
        {
            __result = Control.Shops.Any(i => i.Exists && i.CanUse);

            return false;
        }
    }
}
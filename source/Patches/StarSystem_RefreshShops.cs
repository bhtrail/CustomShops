using BattleTech;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(StarSystem))]
    [HarmonyPatch("RefreshShops")]
    public static class StarSystem_RefreshShops
    {
        [HarmonyPrefix]
        public static bool RefreshShops()
        {
            Control.LogDebug(DInfo.RefreshShop, $"Refreshing Shops OnSystemChange {Control.State.CurrentSystem.Def.Description.Name}");
            foreach (var shop in Control.OnSystemChange)
            {
                Control.LogDebug(DInfo.RefreshShop, $"- [{shop.Name}]");
                try
                {
                    shop.RefreshShop();
                    if (shop is IDefaultShop def_shop)
                    {
                        Control.LogDebug(DInfo.RefreshShop, $"-- total {def_shop.ShopToUse.ActiveInventory.Count} items");
                    }
                }
                catch (Exception e)
                {
                    Control.LogError(e);
                }
            }
            return false;
        }
    }
}

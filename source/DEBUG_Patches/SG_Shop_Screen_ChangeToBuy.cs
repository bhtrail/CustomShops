using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShops.DEBUG_Patches
{
    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("ChangeToBuy")]
    public static class SG_Shop_Screen_ChangeToBuy
    {

        public static void Prefix()
        {
            Control.LogDebug(DInfo.DetailDebug, "ChangeToBuy");
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("AddShopInventory")]
    public static class SG_Shop_Screen_AddShopInventory
    {

        public static void Prefix(Shop shop)
        {
            Control.LogDebug(DInfo.DetailDebug, $"AddShopInventory Prefix - {shop.ActiveInventory.Count} items");
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("AddShopItemToWidget")]
    public static class SG_Shop_Screen_AddShopItemToWidget
    {

        public static void Prefix(ShopDefItem itemDef)
        {
            Control.LogDebug(DInfo.DetailDebug, $"AddShopItemToWidget - {itemDef.GUID}");
        }
    }
}

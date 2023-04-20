using BattleTech;
using BattleTech.UI;

namespace CustomShops
{
    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("AddShopItemToWidget")]
    public static class SH_Shop_Screen_AddShopItemToWidget
    {
        [HarmonyPrefix]
        public static void ReplaceShopDefItem(ref ShopDefItem itemDef)
        {
            itemDef = new TypedShopDefItem(itemDef);
        }
    }
}

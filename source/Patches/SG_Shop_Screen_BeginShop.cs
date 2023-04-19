using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;


namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("BeginShop")]
    public static class SG_Shop_Screen_BeginShop
    {
        [HarmonyPrefix]
        public static bool BeginShop(SG_Shop_Screen __instance)
        {
            UIControler.InitShopWindow(__instance);

            return false;
        }
    }
}

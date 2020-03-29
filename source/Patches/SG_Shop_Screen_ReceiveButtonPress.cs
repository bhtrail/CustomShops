using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using Harmony;

namespace CustomShops.DEBUG_Patches
{
    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("ReceiveButtonPress")]
    public static class SG_Shop_Screen_ReceiveButtonPress
    {
        private static string[] skip = {
            "Capitalism",
            "BUY",
            "SELL",
            "ShopTypeSystem",
            "ShopTypeFaction",
            "ShopTypeBlackMarket"
        };

        public static bool Prefix(string button)
        {
            return !skip.Contains(button);
        }
    }

}

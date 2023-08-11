using System.Linq;
using BattleTech.UI;

namespace CustomShops.DEBUG_Patches;

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

    public static void Prefix(ref bool __runOriginal, string button)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (skip.Contains(button))
        {
            __runOriginal = false;
            return;
        }
        __runOriginal = true;
        return;
    }
}

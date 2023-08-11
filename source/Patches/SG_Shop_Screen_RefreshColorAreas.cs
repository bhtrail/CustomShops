using BattleTech.UI;

namespace CustomShops.Patches;

[HarmonyPatch(typeof(SG_Shop_Screen))]
[HarmonyPatch("RefreshColorAreas")]
public static class SG_Shop_Screen_RefreshColorAreas
{
    [HarmonyPrefix]
    public static void Prefix(ref bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return;
        }

        UIController.RefreshColors(UIController.ActiveShop);
        __runOriginal = false;
        return;
    }
}

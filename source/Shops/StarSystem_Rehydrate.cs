using BattleTech;

namespace CustomShops.Patches;

[HarmonyPatch(typeof(StarSystem))]
[HarmonyPatch("Rehydrate")]
public static class StarSystem_Rehydrate
{
    [HarmonyPrefix]
    public static void OverrideLoadShops(ref bool loadShops)
    {
        loadShops = false;
    }
}

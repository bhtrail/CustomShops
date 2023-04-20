using BattleTech;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("Init")]
    public static class SimGameState_Init
    {
        [HarmonyPostfix]
        public static void OnInit(SimGameState __instance)
        {
            Control.State.Sim = __instance;
        }
    }
}

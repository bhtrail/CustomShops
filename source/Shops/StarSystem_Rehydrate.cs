using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using BattleTech;

namespace CustomShops.Patches
{
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
}

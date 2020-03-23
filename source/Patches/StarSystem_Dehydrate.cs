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
    [HarmonyPatch("Dehydrate")]
    public static class StarSystem_Dehydrate
    {
        [HarmonyPrefix]
        public static void OverrideSaveShops(ref bool saveShops)
        {
            saveShops = false;
        }
    }
}

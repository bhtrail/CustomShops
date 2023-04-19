using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;

namespace CustomShops.Patches
{

    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("SetCurrentSystem")]
    public static class SimGameState_SetCurrentSystem
    {
        [HarmonyPrefix]
        public static void UpdateSystem(StarSystem system)
        {
            Control.State.CurrentSystem = system;
        }
    }
}

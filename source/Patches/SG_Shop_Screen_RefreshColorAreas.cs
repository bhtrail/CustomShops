using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using BattleTech;
using BattleTech.UI;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("RefreshColorAreas")]
    public static class SG_Shop_Screen_RefreshColorAreas
    {
        [HarmonyPrefix]
        public static bool Prefix()
        {
            UIControler.RefreshColors();
            return false;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("RefreshColorAreas")]
    public static class SG_Shop_Screen_RefreshColorAreas
    {
        [HarmonyPrefix]
        public static void Prefix(ref bool __runOriginal)
        {
            if (!__runOriginal) return;
            UIControler.RefreshColors(UIControler.ActiveShop);
            __runOriginal = false;
            return;
        }
    }
}

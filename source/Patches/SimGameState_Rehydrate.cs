using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using BattleTech;
using BattleTech.Save.Test;
using BattleTech.Save;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("Rehydrate")]
    public class SimGameState_Rehydrate
    {
        [HarmonyPostfix]
        public static void LoadShops(GameInstanceSave gameInstanceSave, SimGameState __instance)
        {
            Control.State.CurrentSystem = __instance.CurSystem;
            SerializableReferenceContainer globalReferences = gameInstanceSave.GlobalReferences;
            Control.LogDebug("Loading Shops");
            foreach (var shop in Control.Shops)
            {
                if (shop.NeedSave)
                {
                    var name = "Shop" + shop.Name;
                    Control.LogDebug("- " + shop.Name);
                    try
                    {
                        var Shop = globalReferences.GetItem<Shop>(name);
                        shop.SetLoadedShop(Shop);
                        if (shop.RefreshOnGameLoad)
                            shop.RefreshShop();

                        if (Shop != null)
                            Control.LogDebug("-- " + shop.Name + " Loaded");
                        else
                            Control.LogDebug("-- " + shop.Name + " Notfound");
                    }
                    catch
                    {
                        Control.LogError($"Error finding {name} Create new");
                        shop.SetLoadedShop(null);
                    }
                }
                if (shop.RefreshOnGameLoad || !shop.NeedSave)
                    shop.RefreshShop();
            }
        }
    }
}

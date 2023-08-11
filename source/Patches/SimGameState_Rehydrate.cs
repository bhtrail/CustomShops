using System;
using BattleTech;
using BattleTech.Save.Test;
using BattleTech.Save;

namespace CustomShops.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("Rehydrate")]
public class SimGameState_Rehydrate
{
    [HarmonyPostfix]
    public static void LoadShops(GameInstanceSave gameInstanceSave, SimGameState __instance)
    {
        Control.State.CurrentSystem = __instance.CurSystem;
        Control.State.Sim = __instance;

        SerializableReferenceContainer globalReferences = gameInstanceSave.GlobalReferences;
        Control.LogDebug(DInfo.SaveLoad, "Loading Shops");
        foreach (var shop in Control.Shops)
        {
            if (shop is ISaveShop save)
            {
                var name = "Shop" + shop.Name;
                Control.LogDebug(DInfo.SaveLoad, "- " + shop.Name);
                try
                {
                    var Shop = globalReferences.GetItem<Shop>(name);
                    save.SetLoadedShop(Shop);

                    Control.LogDebug(DInfo.SaveLoad, "-- " + shop.Name + " Loaded");
                    Control.LogDebug(DInfo.SaveLoad, $"-- total {Shop.ActiveInventory.Count} items");
                }
                catch (Exception)
                {
                    Control.LogError($"Error finding {name} Create new");
                    shop.RefreshShop();
                    Control.LogDebug(DInfo.SaveLoad, "-- total " + save.GetShopToSave().ActiveInventory.Count + " items");
                }
            }
            else
            {
                shop.RefreshShop();
            }
        }

        //Control.Log($"Hated: {__instance.Constants.Story.HatedRepShopAdjustment}");
        //Control.Log($"Disliked: {__instance.Constants.Story.DislikedRepShopAdjustment}");
        //Control.Log($"Indifferent: {__instance.Constants.Story.IndifferentRepShopAdjustment}");
        //Control.Log($"Liked: {__instance.Constants.Story.LikedRepShopAdjustment}");
        //Control.Log($"Friendly: {__instance.Constants.Story.FriendlyRepShopAdjustment}");
        //Control.Log($"Honored: {__instance.Constants.Story.HonoredRepShopAdjustment}");
    }
}

using BattleTech;
using BattleTech.Save.Test;

namespace CustomShops.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("Dehydrate")]
public class SimGameState_Dehydrate
{
    [HarmonyPostfix]
    public static void SaveShops(SerializableReferenceContainer references)
    {
        Control.LogDebug(DInfo.SaveLoad, "Saving Shops");
        foreach (var shop in Control.Shops)
        {
            if (shop is ISaveShop save)
            {
                Control.LogDebug(DInfo.SaveLoad, "- " + shop.Name);
                var shop_to_save = save.GetShopToSave();
                if (shop_to_save != null)
                {
                    references.AddItem<Shop>("Shop" + shop.Name, shop_to_save);
                    Control.LogDebug(DInfo.SaveLoad, "-- Saved as Shop" + shop.Name);
                }
                else
                {
                    Control.LogDebug(DInfo.SaveLoad, "-- no shop to save");
                }
            }
        }
    }
}

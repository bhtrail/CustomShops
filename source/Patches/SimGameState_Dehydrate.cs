using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using BattleTech;
using BattleTech.Save.Test;

namespace CustomShops.Patches
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("Dehydrate")]
    public class SimGameState_Dehydrate
    {
        [HarmonyPostfix]
        public static void SaveShops(SerializableReferenceContainer references)
        {
            Control.LogDebug("Saving Shops");
            foreach (var shop in Control.Shops)
            {
                if (shop is ISaveShop save)
                {
                    Control.LogDebug("- " + shop.Name);
                    var shop_to_save = save.GetShopToSave();
                    if (shop_to_save != null)
                    {
                        references.AddItem<Shop>("Shop" + shop.Name, shop_to_save);
                        Control.LogDebug("-- Saved as Shop" + shop.Name);
                    }
                    else
                    {
                        Control.LogDebug("-- no shop to save");
                    }
                }
            }
        }
    }
}

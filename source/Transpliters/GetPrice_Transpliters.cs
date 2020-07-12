using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomShops
{
    public static class GetPrice_Transpliters
    {
        public static Shop.ShopType GetShopType(this Shop shop)
        {
            return Shop.ShopType.System;
        }

        public static int GetPrice(this Shop shop, ShopDefItem item, Shop.PurchaseType purchaseType, Shop.ShopType shopType)
        {
            var price = UIControler.GetPrice(item);
            Control.LogDebug(DInfo.Price, $"get_price for {item.ID}: {price}");
            return price;
        }

        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            var method1 = typeof(Shop).GetProperty("ThisShopType").GetGetMethod();
            var method2 = typeof(Shop).GetMethod("GetPrice");

            foreach (var instruction in instructions)
            {
                if (instruction.operand is MethodInfo operand)
                {
                    if (operand == method1)
                        yield return new CodeInstruction(OpCodes.Call, ((Func<Shop, Shop.ShopType>)GetPrice_Transpliters.GetShopType).Method);
                    else if (operand == method2)
                        yield return new CodeInstruction(OpCodes.Call, ((Func<Shop, ShopDefItem, Shop.PurchaseType, Shop.ShopType, int>)GetPrice_Transpliters.GetPrice).Method);
                    else
                        yield return instruction;
                }
                else
                    yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("CheckHaveNoFunds")]
    public static class SG_Shop_Screen_CheckHaveNoFunds
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }

    [HarmonyPatch(typeof(SG_Shop_Screen))]
    [HarmonyPatch("UpdateMoneySpot")]
    public static class SG_Shop_Screen_UpdateMoneySpot
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }

    [HarmonyPatch(typeof(Shop))]
    [HarmonyPatch("Purchase")]
    public static class Shop_Purchase
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }

    [HarmonyPatch(typeof(InventoryDataObject_ShopFullMech))]
    [HarmonyPatch("RefreshPriceOnWidget")]
    public static class InventoryDataObject_ShopFullMech_RefreshPriceOnWidget
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }

    [HarmonyPatch(typeof(InventoryDataObject_ShopGear))]
    [HarmonyPatch("RefreshPriceOnWidget")]
    public static class InventoryDataObject_ShopGear_RefreshPriceOnWidget
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }

    [HarmonyPatch(typeof(InventoryDataObject_ShopMechPart))]
    [HarmonyPatch("RefreshPriceOnWidget")]
    public static class InventoryDataObject_ShopMechPart_RefreshPriceOnWidget
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }

    [HarmonyPatch(typeof(InventoryDataObject_ShopWeapon))]
    [HarmonyPatch("RefreshPriceOnWidget")]
    public static class InventoryDataObject_ShopWeapon_RefreshPriceOnWidget
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> RepalceGetPrice(IEnumerable<CodeInstruction> instructions)
        {
            return GetPrice_Transpliters.RepalceGetPrice(instructions);
        }

    }
}

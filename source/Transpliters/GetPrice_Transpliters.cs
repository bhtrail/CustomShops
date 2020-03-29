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
            if (UIControler.ActiveShop == null)
            {
                Control.LogError("No Shop to get price!");
                return 1;
            }

            var titem = item as TypedShopDefItem;
            if (titem == null)
            {
                Control.LogError("not typed item");
                return 1;
            }

            int price = 1;
            float discount = 1;
            if (UIControler.ActiveShop is IDefaultPrice)
                price = titem.Type == ShopItemType.MechPart ? titem.Mech.SimGameMechPartCost : titem.Description.Cost;
            else if (UIControler.ActiveShop is ICustomPrice cprice)
                price = cprice.GetPrice(titem);
            else
                Control.LogError("Unknown shop type, cannot get price for " + item.ID);

            if (UIControler.ActiveShop is IDiscountFromFaction faction_discount)
                discount = 1 + Control.State.Sim.GetReputationShopAdjustment(faction_discount.RelatedFaction);
            else if (UIControler.ActiveShop is INoDiscount)
                discount = 1;
            else if (UIControler.ActiveShop is ICustomDiscount cdisc)
                discount = cdisc.GetDiscount();

            return Mathf.CeilToInt(price * discount);
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

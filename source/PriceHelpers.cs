using System;
using BattleTech;
using HBS.Collections;

namespace CustomShops;

public static class PriceHelpers
{
    public static int GetPrice(TypedShopDefItem item)
    {
        var price = item.Type == ShopItemType.MechPart ? item.Mech.SimGameMechPartCost : item.Description.Cost;
        TagSet tagSet = (item.Type == ShopItemType.MechPart || item.Type == ShopItemType.Mech) ? item.Mech.MechTags : item.Component.ComponentTags;

        Control.LogDebug(DInfo.Price, $"Item shop {item.ID} has price = {price} and tag set = '{tagSet}'");
        float priceModifier = 1;
        foreach (var tag in Control.Settings.TagPriceModifiers.Keys)
        {
            if (tagSet.Contains(tag))
            {
                priceModifier = Control.Settings.TagPriceModifiers[tag];
                Control.LogDebug(DInfo.Price, $"price coefficient = {priceModifier} by tag = '{tag}'");
                break;
            }
        }
        return (int)Math.Round(price * priceModifier);
    }
}
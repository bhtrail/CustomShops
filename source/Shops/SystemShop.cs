﻿using UnityEngine;
using BattleTech;
using HBS;
using BattleTech.UI;

namespace CustomShops.Shops;

public class SystemShop : TaggedShop, IDiscountFromFaction, IFillWidgetFromFaction, ISaveShop, ITextIcon, ICustomPrice
{
    public override string Name => "System";
    public override string TabText => "System";
    public override string ShopPanelImage => SG_Stores_StoreImagePanel.STORE_ILLUSTRATION;
    public virtual string SpriteID
    {
        get
        {
            var planet_type = SimGameSpaceController.PlanetType.INVALID_UNSET;
            foreach (var tag in Control.State.CurrentSystem.Def.Tags)
            {
                if (SimGameSpaceController.GetPlanetTypeByTag(tag, out planet_type))
                {
                    break;
                }
            }

            if (planet_type == SimGameSpaceController.PlanetType.INVALID_UNSET)
            {
                return SG_Shop_Screen.BLACKMARKET_ICON;
            }

            return $"planet_climate_{planet_type}";

        }
    }
    public override Color IconColor => Color.white;
    public override Color ShopColor => LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.SystemStoreColor.color;
    public virtual FactionValue RelatedFaction => Control.State.CurrentSystem.OwnerValue;

    public override bool Exists => true;
    public override bool CanUse => Control.State.CurrentSystem != null && Control.State.CurrentSystem.CanUseSystemStore();

    public override bool RefreshOnSystemChange => true;
    public override bool RefreshOnMonthChange => false;
    public override bool RefreshOnOwnerChange => true;
    public override int SortOrder => Control.Settings.SystemShopPriority;


    protected override void UpdateTags()
    {
        Tags = Control.State.CurrentSystem.Def.SystemShopItems;
    }

    public override void SetLoadedShop(Shop shop)
    {
        Tags = Control.State.CurrentSystem.Def.SystemShopItems;
        base.SetLoadedShop(shop);
    }

    public int GetPrice(TypedShopDefItem item) => PriceHelpers.GetPrice(item);
}

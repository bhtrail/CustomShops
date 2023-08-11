using UnityEngine;
using BattleTech;
using HBS;
using BattleTech.UI;

namespace CustomShops.Shops;

public class FactionShop : TaggedShop, ICustomDiscount, IFillWidgetFromFaction, ISaveShop, ISpriteIcon, ICustomPrice
{
    public override string Name => "Faction";
    public override string TabText => RelatedFaction == null ? "ERROR_FACTION" : RelatedFaction.Name;
    public override string ShopPanelImage
    {
        get
        {
            if (RelatedFaction == null)
            {
                return SG_Stores_StoreImagePanel.BLACK_MARKET_ILLUSTRATION;
            }

            return string.IsNullOrEmpty(RelatedFaction.FactionDef.storePanelImage) ?
                SG_Stores_StoreImagePanel.BLACK_MARKET_ILLUSTRATION :
                RelatedFaction.FactionDef.storePanelImage;
        }
    }
    public override int SortOrder => Control.Settings.FactionShopPriority;

    public virtual Sprite Sprite
    {
        get
        {
            if (!Exists)
            {
                return null;
            }

            var owner = RelatedFaction;
            return owner != null ? owner.FactionDef.GetSprite() : null;
        }
    }
    public override Color IconColor
    {
        get
        {
            if (!Exists)
            {
                return Color.white;// LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
            }

            var owner = Control.State.CurrentSystem.Def.FactionShopOwnerValue;
            if (owner == null)
            {
                return Color.white; // LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
            }

            return Color.white; //owner.FactionDef.GetFactionStoreColor(out var color) ? color : LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
        }
    }
    public override Color ShopColor
    {
        get
        {
            if (!Exists)
            {
                return LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
            }

            var owner = Control.State.CurrentSystem.Def.FactionShopOwnerValue;
            if (owner == null)
            {
                return LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
            }

            return owner.FactionDef.GetFactionStoreColor(out var color) ? color : LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
        }
    }

    public virtual FactionValue RelatedFaction => Control.State.CurrentSystem.Def.FactionShopOwnerValue;

    public override bool Exists => RelatedFaction!= null && !RelatedFaction.IsInvalidUnset && Control.State.CurrentSystem != null && Control.State.CurrentSystem.Def.FactionShopItems != null;
    public override bool CanUse => Control.Settings.DEBUG_FactionShopAlwaysAvailable || (RelatedFaction != null && Control.State.Sim.IsFactionAlly(RelatedFaction));
    public override bool RefreshOnSystemChange => true;
    public override bool RefreshOnMonthChange => false;
    public override bool RefreshOnOwnerChange => true;



    protected override void UpdateTags()
    {
        Tags = Control.State.CurrentSystem.Def.FactionShopItems;
    }
    public override void SetLoadedShop(Shop shop)
    {
        Tags = Control.State.CurrentSystem.Def.FactionShopItems;
        base.SetLoadedShop(shop);
    }

    public float GetDiscount(TypedShopDefItem item)
    {
        return 1 + Control.Settings.FactionShopAdjustment;
    }

    public int GetPrice(TypedShopDefItem item) => throw new System.NotImplementedException();
}

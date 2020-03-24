using UnityEngine;
using BattleTech;
using HBS;
using BattleTech.UI;

namespace CustomShops.Shops
{
    public class FactionShop : TaggedShop, IDiscountFromFaction, IFillWidgetFromFaction, ISaveShop, ISpriteIcon
    {
        public override string Name => "Faction";
        public override string TabText => RelatedFaction == null ? "ERROR_FACTION" : RelatedFaction.Name;
        public override string HeaderText => "Faction";
        public override string ShopPanelImage
        {
            get
            {
                if(RelatedFaction == null)
                    return SG_Stores_StoreImagePanel.BLACK_MARKET_ILLUSTRATION;

                return string.IsNullOrEmpty(RelatedFaction.FactionDef.storePanelImage) ?
                    SG_Stores_StoreImagePanel.BLACK_MARKET_ILLUSTRATION :
                    RelatedFaction.FactionDef.storePanelImage;
            }
        }

        public virtual Sprite Sprite
        {
            get
            {
                if (!Exists)
                    return null;

                var owner = Control.State.CurrentSystem.Def.FactionShopOwnerValue;
                if (owner == null)
                    return null;

                return owner.FactionDef.GetSprite();
            }
        }
        public override Color IconColor
        {
            get
            {
                if (!Exists)
                    return LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
                var owner = Control.State.CurrentSystem.Def.FactionShopOwnerValue;
                if (owner == null)
                    return LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
                return owner.FactionDef.GetFactionStoreColor(out var color) ? color : LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
            }
        }
        public override Color ShopColor
        {
            get
            {
                if (!Exists)
                    return LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
                var owner = Control.State.CurrentSystem.Def.FactionShopOwnerValue;
                if (owner == null)
                    return LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
                return owner.FactionDef.GetFactionStoreColor(out var color) ? color : LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.FactionStoreColor.color;
            }
        }
        public FactionValue RelatedFaction => Control.State.CurrentSystem.Def.FactionShopOwnerValue;

        public override bool Exists => Control.State.CurrentSystem == null ? false : Control.State.CurrentSystem.Def.FactionShopItems != null;
        public override bool CanUse => true; //Control.State.CurrentSystem == null ? false : Control.State.Sim.IsFactionAlly(Control.State.CurrentSystem.Def.FactionShopOwnerValue);
        public override bool RefreshOnSystemChange => true;
        public override bool RefreshOnMonthChange => false;
        public override bool RefreshOnOwnerChange => true;



        public override void Initilize()
        {
            Tags = Control.State.CurrentSystem.Def.FactionShopItems;
            base.Initilize();
        }
        public override void SetLoadedShop(Shop shop)
        {
            Tags = Control.State.CurrentSystem.Def.FactionShopItems;
            base.SetLoadedShop(shop);
        }

    }
}

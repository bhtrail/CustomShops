using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BattleTech;
using HBS;
using BattleTech.UI;

namespace CustomShops.Shops
{
    public class SystemShop : TaggedShop
    {
        public override string Name => "System";
        public override Sprite Sprite => Control.State.SystemShopSprite;
        public override Color IconColor => LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.SystemStoreColor.color;
        public override Color ShopColor => LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.SystemStoreColor.color;

        public override bool Exists => true;
        public override bool CanUse => Control.State.CurrentSystem == null ? false : Control.State.CurrentSystem.CanUseSystemStore();



        public override bool RefreshOnSystemChange => true;
        public override bool RefreshOnMonthChange => false;
        public override bool RefreshOnOwnerChange => true;
        public override bool RefreshOnGameLoad => false;
        public override bool NeedSave => true;

        public override void Initilize()
        {
            Tags = Control.State.CurrentSystem.Def.SystemShopItems;
            base.Initilize();
        }

        public override void SetLoadedShop(Shop shop)
        {
            Tags = Control.State.CurrentSystem.Def.BlackMarketShopItems;
            Initilize();
        }
    }
}

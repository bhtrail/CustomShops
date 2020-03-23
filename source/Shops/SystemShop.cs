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
    public class SystemShop : TaggedShop, IDiscountFromFaction, IFillWidgetFromFaction
    {
        public override string Name => "System";
        public override string TabText => "System";
        public override string HeaderText => Control.State.CurrentSystem.Def.Description.Name;
        public override string ShopPanelImage => SG_Stores_StoreImagePanel.STORE_ILLUSTRATION;
        public override Sprite Sprite
        {
            get
            {
                var planet_type = SimGameSpaceController.PlanetType.INVALID_UNSET;
                foreach (var tag in Control.State.CurrentSystem.Def.Tags)
                    if (SimGameSpaceController.GetPlanetTypeByTag(tag, out planet_type))
                        break;
                if (planet_type == SimGameSpaceController.PlanetType.INVALID_UNSET)
                    return Control.State.SystemShopSprite;

                if (planet_type != climate)
                {
                    var id = $"planet_climate_{planet_type}";
                    var loadrequest = Control.State.Sim.DataManager.CreateLoadRequest();
                    loadrequest.AddLoadRequest<Sprite>(BattleTechResourceType.Sprite,id, null);
                    loadrequest.ProcessRequests();
                    sprite = Control.State.Sim.DataManager.GetObjectOfType<Sprite>(id, BattleTechResourceType.Sprite);
                }
                return sprite;
            }
        }
        public override Color IconColor => LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.SystemStoreColor.color;
        public override Color ShopColor => LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.SystemStoreColor.color;
        public FactionValue RelatedFaction => Control.State.CurrentSystem.OwnerValue;

        public override bool Exists => true;
        public override bool CanUse => Control.State.CurrentSystem == null ? false : Control.State.CurrentSystem.CanUseSystemStore();

        private SimGameSpaceController.PlanetType climate = SimGameSpaceController.PlanetType.INVALID_UNSET;
        private Sprite sprite = null;

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

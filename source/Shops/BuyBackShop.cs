using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using UnityEngine;
using BattleTech.UI;
using Harmony;

namespace CustomShops
{
    public class BuyBackShop : IShopDescriptor, ISaveShop, IDefaultShop, ICustomFillWidget, ITextIcon
    {
        private Traverse shopT;

        public Shop ShopToUse { get; private set; }

        public string Name => "BuyBack";

        public string TabText => "Buyback";

        public string ShopPanelImage => SG_Stores_StoreImagePanel.STORE_ILLUSTRATION;

        public Color IconColor => Color.green;

        public Color ShopColor => Color.green;

        public bool Exists => true;

        public bool CanUse => true;

        public bool RefreshOnSystemChange => true;

        public bool RefreshOnMonthChange => false;

        public bool RefreshOnOwnerChange => false;

        public string SpriteID => "uixTxrIcon_mrb-star"; // "uixTxrIcon_planet"

        public void FillFactionWidget(MiniFactionPanelHelper helper)
        {
            Control.State.Sim.RequestItem<Sprite>(SpriteID, (sprite) => helper.FactionIcon.sprite = sprite, BattleTechResourceType.Sprite);
            helper.HideRatingIcons();
            helper.ReputationBonusText.SetText("Buy items back");
        }

        public Shop GetShopToSave()
        {
            return ShopToUse;
        }

        public void RefreshShop()
        {
            if(ShopToUse == null)
            {
                ShopToUse = new Shop();
                shopT = new Traverse(ShopToUse);
            }

            shopT.Field<SimGameState>("Sim").Value = UnityGameInstance.BattleTechGame.Simulation;
            shopT.Field<StarSystem>("system").Value = Control.State.CurrentSystem;
            if (ShopToUse.ItemCollections == null)
                shopT.Property<List<ItemCollectionDef>>("ItemCollections").Value = new List<ItemCollectionDef>();
            else
                ShopToUse.ItemCollections.Clear();
        }

        public void SetLoadedShop(Shop shop)
        {
            this.ShopToUse = shop;
        }
    }
}

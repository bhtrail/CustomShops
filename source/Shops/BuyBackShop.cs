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
    public class BuyBackShop : IShopDescriptor, ISaveShop, IDefaultShop, ICustomFillWidget, ITextIcon, INoDiscount, ICustomPrice, ISellShop
    {
        private Traverse shopT;
        private Color PanelColor = new Color(0, 0.2f, 0);

        public Shop ShopToUse { get; private set; }

        public string Name => "BuyBack";

        public string TabText => "Buyback";

        public string ShopPanelImage => SG_Stores_StoreImagePanel.STORE_ILLUSTRATION;

        public Color IconColor => Color.green;

        public Color ShopColor => PanelColor;

        public bool Exists => true;

        public bool CanUse => true;

        public bool RefreshOnSystemChange => true;

        public bool RefreshOnMonthChange => false;

        public bool RefreshOnOwnerChange => false;

        public string SpriteID => "customshops_cbill";// "uixTxrIcon_mrb-star"; // "uixTxrIcon_planet"

        public int SortOrder => Control.Settings.BuyBackShopPriority;

        public int SellPriority => CustomShopsSettings.BUYBACK_SHOP_PRIORITY;

        public void FillFactionWidget(ShopScreenHelper helper)
        {
            try
            {
                var mhelper = helper.MiniWidgetHelper;
                Control.State.Sim.RequestItem<Sprite>(SpriteID, (sprite) => mhelper.FactionIcon.sprite = sprite, BattleTechResourceType.Sprite);
                mhelper.HideRatingIcons();
                mhelper.ReputationBonusText.SetText("Buy items back");
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
        }

        public int GetPrice(TypedShopDefItem item)
        {
            var price = item.Type == ShopItemType.MechPart ? item.Mech.SimGameMechPartCost : item.Description.Cost;

            return Mathf.CeilToInt(price * (
                item.IsDamaged
                ? Control.State.Sim.Constants.Finances.ShopSellDamagedModifier 
                : Control.State.Sim.Constants.Finances.ShopSellModifier));
        }

        public Shop GetShopToSave()
        {
            return ShopToUse;
        }

        public void RefreshShop()
        {
            if (ShopToUse == null)
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
            shopT = new Traverse(ShopToUse);
        }

        public void AddItemToShop(ShopDefItem item, int count)
        {
            try
            {
                if (item.Type == ShopItemType.Mech)
                {
                    Control.LogDebug(DInfo.BuyBack, "- cannot add mech to buyback, skip");
                    return;
                }

                var res = new ItemCollectionResultGenerator(Control.State.Sim);
                var itemdef = new ShopDefItem(item);
                itemdef.Count = count;
                res.InsertShopDefItem(itemdef, ShopToUse.ActiveInventory);

            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
        }

        public bool OnSellItem(ShopDefItem item, int num)
        {
            if (Control.Settings.BuyBackShop)
            {
                Control.LogDebug(DInfo.BuyBack, $"Addiing {item.ID} to buy back shop");
                Control.BuyBack.AddItemToShop(item, num);
                return true;
            }
            return false;
        }
    }
}

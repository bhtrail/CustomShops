using System;
using System.Collections.Generic;
using BattleTech;
using UnityEngine;
using BattleTech.UI;

namespace CustomShops;

public class BuyBackShop : IShopDescriptor, ISaveShop, IDefaultShop, ICustomFillWidget, ITextIcon, INoDiscount, ICustomPrice, ISellShop, ICustomPurchase
{
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
        }
        
        ShopToUse.Sim = UnityGameInstance.BattleTechGame.Simulation;
        ShopToUse.system = Control.State.CurrentSystem;
        if (ShopToUse.ItemCollections == null)
        {
            ShopToUse.ItemCollections = new List<ItemCollectionDef>();
        }
        else
        {
            ShopToUse.ItemCollections.Clear();
        }

        ShopToUse.ActiveInventory.Clear();
    }

    public void SetLoadedShop(Shop shop)
    {
        ShopToUse = shop;
    }

    private void AddItemToShop(ShopDefItem item, int count)
    {
        try
        {
            bool ismech = item.Type == ShopItemType.Mech;
            ShopDefItem item_to_add = new ShopDefItem(
                ismech ? item.ID.Replace("chassisdef", "mechdef") : item.ID,
                item.Type,
                item.DiscountModifier,
                count,
                item.IsInfinite,
                item.IsDamaged,
                item.SellCost
                );

            var res = new ItemCollectionResultGenerator(Control.State.Sim);
            res.InsertShopDefItem(item_to_add, ShopToUse.ActiveInventory);

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

    public bool Purchase(ShopDefItem item, int quantity)
    {
        if (item.Type == ShopItemType.Mech)
        {
            var shop_item = ShopToUse.ActiveInventory.Find((ShopDefItem cachedItem) => cachedItem.ID == item.ID && cachedItem.Type == item.Type);
            if (shop_item == null)
            {
                Control.LogError($"Shop don't contain {item.ID} to purchase");
                return false;
            }
            if (!item.IsInfinite)
            {
                shop_item.Count -= quantity;
                if (shop_item.Count < 1)
                {
                    ShopToUse.ActiveInventory.Remove(shop_item);
                }
            }
            int price = UIController.GetPrice(item);

            Control.State.Sim.AddFunds(-price * quantity, null, true, true);
            var mech = Control.State.Sim.DataManager.MechDefs.Get(item.ID);

            for (int i = 0; i < quantity; i++)
            {
                Control.State.Sim.AddItemStat(mech.Chassis.Description.Id, mech.GetType(), false);
            }

            return true;
        }

        return UIController.DefaultPurshase(this, item, quantity);
    }

}

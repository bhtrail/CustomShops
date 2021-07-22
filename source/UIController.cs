using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
using HBS;
using UnityEngine;
using UnityEngine.UI;

namespace CustomShops
{
    public static class UIControler
    {
        private static SG_Shop_Screen ShopScreen;
        public static ShopScreenHelper ShopHelper;
        private static Dictionary<string, StoreButton> Buttons;
        public static IShopDescriptor ActiveShop { get; private set; }

        internal static void InitShopWindow(SG_Shop_Screen shop_screen)
        {
            Control.LogDebug(DInfo.ShopInterface, "Enter Shop Screen");
            if (shop_screen != ShopScreen)
                SetupButtons(shop_screen);

            StoreButton active = null;
            Control.LogDebug(DInfo.ShopInterface, "- SetupButtons");
            foreach (var button in Buttons.Values)
            {
                try
                {
                    var shop = button.Shop;
                    Control.LogDebug(DInfo.ShopInterface, $"-- [{shop.Name}]");
                    Control.LogDebug(DInfo.ShopInterface, $"--- Exists:{shop.Exists} CanUse:{shop.CanUse}");

                    if (shop.Exists)
                    {
                        button.ButtonHolder.SetActive(true);

                        if (shop is ISpriteIcon spr_icon)
                        {
                            Control.LogDebug(DInfo.ShopInterface, $"--- sprite image - set");
                            button.Button.SetImageAndText(spr_icon.Sprite, shop.TabText);
                            Control.LogDebug(DInfo.ShopInterface, $"--- set icon color to  r{shop.IconColor.r:0.00} g{shop.IconColor.g:0.00} b{shop.IconColor.b:0.00}");
                            button.Icon.color = shop.IconColor;

                        }
                        else if (shop is ITextIcon txt_icon)
                        {
                            var id = txt_icon.SpriteID;
                            Control.LogDebug(DInfo.ShopInterface, $"--- txt image - {id} - request");
                            Control.State.Sim.RequestItem<Sprite>(
                               id,
                               (sprite) =>
                               {
                                   Control.LogDebug(DInfo.ShopInterface, $"--- button image {id} received ");
                                   button.Button.SetImageAndText(sprite, shop.TabText);
                                   Control.LogDebug(DInfo.ShopInterface, $"--- set icon color to  r{shop.IconColor.r:0.00} g{shop.IconColor.g:0.00} b{shop.IconColor.b:0.00}");
                                   button.Icon.color = shop.IconColor;
                                   Control.LogDebug(DInfo.ShopInterface, $"--- set");
                               },
                               BattleTechResourceType.Sprite
                               );
                        }

                        if (shop.CanUse)
                        {
                            button.Button.SetState(ButtonState.Enabled);
                            button.Overlay.SetActive(false);
                            if (active == null)
                                active = button;
                        }
                        else
                        {
                            button.Button.SetState(ButtonState.Disabled);
                            button.Overlay.SetActive(true);
                        }
                    }
                    else
                    {
                        button.ButtonHolder.SetActive(false);
                    }
                }
                catch (Exception e)
                {
                    Control.LogError(e);
                }
            }
            if (active != null)
            {
                active.Button.ForceRadioSetSelection();
                ActiveShop = active.Shop;
                ShopScreen.StartCoroutine(SwitchAtEndOfFrame());
            }
        }

        private static IEnumerator SwitchAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            TabSelected(ActiveShop);
            yield break;
        }


        private static bool show_full_price_debug = false;
        public static int GetPrice(ShopDefItem item)
        {
            if (UIControler.ActiveShop == null)
            {
                Control.LogError("No Shop to get price!");
                return 1;
            }

            var titem = item as TypedShopDefItem;
            if (titem == null)
            {
                titem = new TypedShopDefItem(item);
                return 1;
            }

            int price = 1;
            float discount = 1;
            if (UIControler.ActiveShop is IDefaultPrice)
                if (titem.Type == ShopItemType.MechPart)
                    price = titem.Mech.SimGameMechPartCost;
                else if (titem.Type == ShopItemType.Mech)
                {
                    if (Control.Settings.CheckFakeVehicle &&
                        titem.Mech.Chassis.ChassisTags.Contains(Control.Settings.FakeVehicleTag))
                        price = titem.Mech.Chassis.Description.Cost;
                    else
                        price = titem.Mech.Description.Cost;
                }
                else
                    price = titem.Description.Cost;

            else if (UIControler.ActiveShop is ICustomPrice cprice)
                price = cprice.GetPrice(titem);
            else
                Control.LogError("Unknown shop type, cannot get price for " + item.ID);

            if (Control.Settings.NoDiscountOnLoan && price < 0)
                discount = 1;
            else if (UIControler.ActiveShop is IDiscountFromFaction faction_discount)
                discount = 1 + Control.State.Sim.GetReputationShopAdjustment(faction_discount.RelatedFaction);
            else if (UIControler.ActiveShop is INoDiscount)
                discount = 1;
            else if (UIControler.ActiveShop is ICustomDiscount cdisc)
                discount = cdisc.GetDiscount(titem);

            Control.LogDebug(DInfo.Price, $"Total price for {titem.Description.Id} is {price:0000}*{discount:0.00} = {Mathf.CeilToInt(price * discount)}");

            return Mathf.CeilToInt(price * discount);
        }
        private static void SetupButtons(SG_Shop_Screen shop_screen)
        {
            try
            {
                Control.LogDebug(DInfo.ShopInterface, "- First Time enter. Start to setup");
                ShopScreen = shop_screen;
                ShopHelper = new ShopScreenHelper(ShopScreen);
                Buttons = new Dictionary<string, StoreButton>();


                var store_button = ShopHelper.SystemStoreButtonHoldingObject;
                var radio_set = ShopHelper.SystemStoreButtonHoldingObject.transform.parent.GetComponent<HBSRadioSet>();

                Control.LogDebug(DInfo.ShopInterface, "-- Create buttons");
                foreach (var shop in Control.Shops.OrderBy(i => i.SortOrder))
                {
                    Control.LogDebug(DInfo.ShopInterface, $"--- {shop.Name}");
                    var button = new StoreButton(store_button, shop);
                    Buttons.Add(shop.Name, button);
                }

                Control.LogDebug(DInfo.ShopInterface, "-- Hide original buttons");
                ShopHelper.SystemStoreButtonHoldingObject.SetActive(false);
                ShopHelper.FactionStoreButtonHoldingObject.SetActive(false);
                ShopHelper.BlackMarketStoreButtonHoldingObject.SetActive(false);

                Control.LogDebug(DInfo.ShopInterface, "-- Setup radio set");
                radio_set.ClearRadioButtons();

                foreach (var pair in Buttons)
                    radio_set.AddButtonToRadioSet(pair.Value.Button);

                radio_set.defaultButton = Buttons.Values.First().Button;

                Control.LogDebug(DInfo.ShopInterface, "-- Replace Buy/Sell buttons");

                ShopHelper.BuyTabButton.OnClicked.RemoveAllListeners();
                ShopHelper.SellTabButton.OnClicked.RemoveAllListeners();
                ShopHelper.BuyButton.OnClicked.RemoveAllListeners();

                ShopHelper.BuyTabButton.OnClicked.AddListener(OnBuyTabPress);
                ShopHelper.SellTabButton.OnClicked.AddListener(OnSellTabPress);
                ShopHelper.BuyButton.OnClicked.AddListener(OnBuySellPress);
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
            Control.LogDebug(DInfo.ShopInterface, "-- done!");

        }
        private static void OnBuySellPress()
        {
            try
            {
                Control.LogDebug(DInfo.ShopActions, "Buy/Sell");
                var selected = ShopHelper.selectedController;
                if (selected == null)
                {
                    Control.LogDebug(DInfo.ShopActions, "- nothing selected, return");
                    return;
                }

                if (ShopHelper.isInBuyingState)
                {
                    Control.LogDebug(DInfo.ShopActions, "- Buy");
                    ProcessBuy(selected);
                }
                else
                {
                    Control.LogDebug(DInfo.ShopActions, "- Sell");
                    ProcessSell(selected);
                }
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
        }
        private static void ProcessSell(InventoryDataObject_SHOP selected)
        {
            if (selected == null)
            {
                Control.LogDebug(DInfo.ShopActions, "-- nothing to sell, return");

                return;
            }

            var price = selected.GetCBillValue();
            if (selected.quantity == 1 || !Control.Settings.AllowMultiSell)
            {
                Control.LogDebug(DInfo.ShopActions, $"-- selling 1x{selected.GetName()}");
                if (Control.Settings.ShowConfirm && price > Control.Settings.ConfirmLowLimit)
                {
                    GenericPopupBuilder.Create("Confirm?", $"Sell {selected.GetName()} for {price}?")
                        .AddButton("Cancel", null, true, null)
                        .AddButton("Accept", () => OnSellItems(1), true, null)
                        .CancelOnEscape()
                        .AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true)
                        .Render();
                }
                else
                {
                    OnSellItems(1);
                }
            }
            else
            {
                Control.LogDebug(DInfo.ShopActions, $"-- show multisell dialog");
                SG_Stores_MultiPurchasePopup_Handler.StartDialog("Sell", selected.shopDefItem,
                        selected.GetName(), selected.quantity, price, OnSellItems, null);
            }
        }
        private static void OnSellItems(int num)
        {
            var selected = ShopHelper.selectedController;
            Control.LogDebug(DInfo.ShopActions, $"-- OnSellItems {num}x{selected.GetName()}");
            var shop_def = selected.shopDefItem;
            if (selected.quantity < num)
                num = selected.quantity;
            if (num <= 0)
            {
                Control.LogDebug(DInfo.ShopActions, $"--- num<=0 return");
                return;
            }
            int sold = SellItem(shop_def, num);

            if (sold > 0)
            {

                Control.LogDebug(DInfo.ShopActions, $"--- sold, process messages");
                SimGamePurchaseMessage message = new SimGamePurchaseMessage(shop_def, shop_def.SellCost, SimGamePurchaseMessage.TransactionType.Sell);
                Control.State.Sim.MessageCenter.PublishMessage(message);
                selected.ModifyQuantity(-num);
                if (selected.quantity <= 0)
                {
                    ShopHelper.inventoryWidget.RemoveDataItem(selected);
                    selected.Pool();
                    ShopHelper.selectedController = null;
                }
                ShopHelper.inventoryWidget.RefreshInventoryList();
                ShopScreen.UpdateMoneySpot();
                ShopHelper.triggerIronManAutoSave = true;

                foreach (var s in Control.SaleShops)
                {
                    if (s is IShopDescriptor sd && sd.CanUse)
                        if (s.OnSellItem(shop_def, sold))
                            break;

                }
            }
            else
                Control.LogDebug(DInfo.ShopActions, $"--- not sold");
        }
        private static int SellItem(ShopDefItem item, int num)
        {
            Control.LogDebug(DInfo.ShopActions, $"--- SellItem {num}x{item.ID}");
            int sell_cost = item.SellCost;
            if (item.Type == ShopItemType.MechPart)
            {
                Control.LogDebug(DInfo.ShopActions, $"---- mechpart - not sold");
                return 0;
            }
            Type type2;
            if (item.Type == ShopItemType.Mech || item.Type == ShopItemType.Chassis_DEPRECATED)
            {
                type2 = typeof(MechDef);
            }
            else
            {
                type2 = SimGameState.GetTypeFromComponent(Shop.ShopItemTypeToComponentType(item.Type));
            }
            var avaliable = Control.State.Sim.GetItemCount(item.ID, type2,
                item.IsDamaged ? SimGameState.ItemCountType.DAMAGED_ONLY : SimGameState.ItemCountType.UNDAMAGED_ONLY);
            Control.LogDebug(DInfo.ShopActions, $"---- {type2}, avaliable: {avaliable}");

            if (avaliable < num)
                num = avaliable;
            if (num == 0)
                return 0;
            for (int i = 0; i < num; i++)
                Control.State.Sim.RemoveItemStat(item.ID, type2, item.IsDamaged);
            Control.State.Sim.AddFunds(sell_cost * num, "Store", true, true);
            Control.LogDebug(DInfo.ShopActions, $"---- done");
            return num;
        }

        private static void ProcessBuy(InventoryDataObject_SHOP selected)
        {
            var item_type = selected.GetItemType();

            if (Control.State.Sim.InMechLabStore() && (
                item_type == MechLabDraggableItemType.StorePart ||
                item_type == MechLabDraggableItemType.SalvagePart
                ))
            {
                Control.LogDebug(DInfo.ShopActions, "- cannot buy mech and parts in mechlab, return");
                return;
            }

            //if (ActiveShop is IDefaultShop def_shop)
            ////{
            //    Control.LogDebug(DInfo.ShopActions, "- IDefaultShop");
            //    var shop = def_shop.ShopToUse;
            //    if (shop == null)
            //    {
            //        Control.LogDebug(DInfo.ShopActions, "-- no shop, return");
            //        return;
            //    }
            int price = GetPrice_Transpliters.GetPrice(null, selected.shopDefItem, Shop.PurchaseType.Normal, Shop.ShopType.System);
            var money = Control.State.Sim.Funds;
            var max_to_buy = money / price;
            if (!selected.shopDefItem.IsInfinite && max_to_buy > selected.quantity)
                max_to_buy = selected.quantity;

            Control.LogDebug(DInfo.ShopActions, $"-- money:{money} price:{price} num:{selected.quantity} max:{max_to_buy}");

            if (max_to_buy == 1 || !Control.Settings.AllowMultiBuy)
            {
                if (Control.Settings.ShowConfirm && price >= Control.Settings.ConfirmLowLimit)
                {
                    GenericPopupBuilder.Create("Confirm?", $"Purchase {selected.GetName()} for {price}?")
                        .AddButton("Cancel", null, true, null)
                        .AddButton("Accept", () => OnBuyItem(1), true, null)
                        .CancelOnEscape()
                        .AddFader(new UIColorRef?(LazySingletonBehavior<UIManager>.Instance.UILookAndColorConstants.PopupBackfill), 0f, true)
                        .Render();
                }
                else
                {
                    OnBuyItem(1);
                }
            }
            else
            {
                SG_Stores_MultiPurchasePopup_Handler.StartDialog("Buy", selected.shopDefItem,
                    selected.GetName(), max_to_buy, price, OnBuyItem, null);
            }
            //}
            //else if (ActiveShop is IListShop shop)
            //{ 

            //}
            //else
            //{
            //    Control.LogError("- unknown type of shop, return");
            //}
        }
        private static void OnBuyItem(int num)
        {
            var selected = ShopHelper.selectedController;
            var shop_def = selected.shopDefItem;

            Control.LogDebug(DInfo.ShopActions, $"-- {num}x{shop_def.ID}");

            if (selected == null)
            {
                Control.LogError("OnBuyItem null item, cancel");
                return;
            }
            var id = selected.GetId();
            if (!shop_def.IsInfinite && num > selected.quantity)
            {
                Control.LogDebug(DInfo.ShopActions, $"--- {num} > {selected.quantity}, adjucting");
                num = selected.quantity;
            }


            if (BuyItem(shop_def, num))
            {

                if (ActiveShop is IForceRefresh)
                {
                    Control.LogDebug(DInfo.ShopActions, $"--- refresh shop - forced reload");
                    ForceRefresh();
                }
                else
                {
                    Control.LogDebug(DInfo.ShopActions, $"--- refresh shop - default");
                    if (!shop_def.IsInfinite)
                        if (num < selected.quantity)
                        {
                            Control.LogDebug(DInfo.ShopActions, $"---- {num} < {selected.quantity} - reducing");
                            selected.ModifyQuantity(-num);
                        }
                        else
                        {

                            Control.LogDebug(DInfo.ShopActions, $"--- {num} >= {selected.quantity} - removing");
                            selected.quantity = 1;
                            ShopHelper.inventoryWidget.RemoveDataItem(selected);
                            selected.Pool();
                            ShopHelper.selectedController = null;

                        }
                }

                Control.LogDebug(DInfo.ShopActions, $"--- PlayVO");
                if (ShopHelper.canPlayVO)
                {
                    ShopHelper.canPlayVO = false;
                    ShopItemType type = shop_def.Type;
                    string text;
                    if (type != ShopItemType.Weapon)
                    {
                        if (type == ShopItemType.Mech)
                        {
                            text = "store_newmechs";
                        }
                        else
                        {
                            text = "store_newequipment";
                        }
                    }
                    else
                    {
                        text = "store_newweapons";
                    }
                    if (!string.IsNullOrEmpty(text))
                    {
                        AudioEventManager.PlayAudioEvent("audioeventdef_simgame_vo_barks", text, WwiseManager.GlobalAudioObject, null);
                    }
                    ShopScreen.StartCoroutine(PurchaseVOCooldown(5f));
                }
            }

            ShopHelper.inventoryWidget.RefreshInventoryList();

            ShopScreen.UpdateMoneySpot();
            ShopScreen.RefreshAllMoneyListings();
            if (shop_def.Type == ShopItemType.MechPart)
            {
                ShopScreen.OnItemSelected(ShopHelper.inventoryWidget.GetSelectedViewItem());
            }
            ShopHelper.triggerIronManAutoSave = true;
        }

        public static bool DefaultPurshase(IDefaultShop shop, ShopDefItem item, int quantity)
        {
            var ushop = shop.ShopToUse;
            var shop_item = ushop.ActiveInventory.Find((ShopDefItem cachedItem) => cachedItem.ID == item.ID && cachedItem.Type == item.Type);
            if (shop_item == null)
            {
                Control.LogError($"Shop dont containg {item.ID} to purshase");
                return false;
            }
            if (!item.IsInfinite)
            {
                shop_item.Count -= quantity;
                if (shop_item.Count < 1)
                {
                    ushop.ActiveInventory.Remove(shop_item);
                }
            }
            int price = GetPrice(item);

            Control.State.Sim.AddFunds(-price * quantity, null, true, true);
            for (int i = 0; i < quantity; i++)
                Control.State.Sim.AddFromShopDefItem(shop_item, false, price, SimGamePurchaseMessage.TransactionType.Purchase);
            return true;
        }


        public static bool DefaultPurshase(IListShop shop, ShopDefItem item, int quantity)
        {
            var list = shop.Items;
            var item_to_buy = list.FirstOrDefault(i => i.IsDamaged == item.IsDamaged && item.ID == i.ID);
            if (item_to_buy == null)
            {
                Control.LogError($"Cannot find {item.ID} to buy");
                return false;
            }

            if (!item_to_buy.IsInfinite)
            {
                if (item_to_buy.Count < quantity)
                    item_to_buy.Count -= quantity;
                else
                    list.Remove(item_to_buy);
            }

            int price = GetPrice(item);

            Control.State.Sim.AddFunds(-price * quantity, null, true, true);
            for (int i = 0; i < quantity; i++)
                Control.State.Sim.AddFromShopDefItem(item, false, price, SimGamePurchaseMessage.TransactionType.Purchase);
            return true;
        }

        private static bool BuyItem(ShopDefItem item, int quantity)
        {
            Control.LogDebug(DInfo.ShopActions, $"---  BuyItem");

            if (ActiveShop is ICustomPurshase cus_shop)
            {
                return cus_shop.Purshase(item, quantity);
            }
            else if (ActiveShop is IDefaultShop def_shop)
            {
                Control.LogDebug(DInfo.ShopActions, $"---- IDefaultShop");
                return DefaultPurshase(def_shop, item, quantity);
            }
            else if (ActiveShop is IListShop l_shop)
            {
                Control.LogDebug(DInfo.ShopActions, $"---- IListShop");
                return DefaultPurshase(l_shop, item, quantity);
            }


            Control.LogError("- unknown type of shop, return");
            return false;
        }

        private static IEnumerator PurchaseVOCooldown(float duration)
        {
            yield return new WaitForSeconds(duration);
            ShopHelper.canPlayVO = true;
            yield break;
        }

        private static void OnSellTabPress()
        {
            try
            {
                Control.LogDebug(DInfo.ShopActions, "Switch To Sell");
                ShopScreen.ChangeToSell();
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
        }

        private static void OnBuyTabPress()
        {
            try
            {
                Control.LogDebug(DInfo.ShopActions, "Switch To Buy");
                TabSelected(ActiveShop);
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
        }

        public static void RefreshColors(IShopDescriptor shop)
        {
            if (shop != null)
            {
                var color = shop.ShopColor;
                foreach (var crtrack in ShopHelper.ColorAffectors)
                    crtrack.OverrideWithColor(color);

                color.a *= SG_Shop_Screen.CENTER_BG_ALPHASCALAR;
                ShopHelper.LargeBGFillColor.OverrideWithColor(color);
            }
        }

        internal static void TabSelected(IShopDescriptor shop)
        {
            Control.LogDebug(DInfo.TabSwitch, $"Pressed {shop.Name}");
            ActiveShop = shop;

            try
            {
                SwitchAndInit(shop);
                RefreshColors(shop);
                FillInData(shop);
            }
            catch (Exception e)
            {
                Control.LogError("Error while switching shop", e);
            }
        }

        private static void SwitchAndInit(IShopDescriptor shop)
        {
            Control.LogDebug(DInfo.TabSwitch, $"- fill shop");
            if (shop == null)
                return;

            if (shop is IDefaultShop def_shop)
            {
                Control.LogDebug(DInfo.TabSwitch, $"-- IDefaultShop");
                ShopScreen.ChangeToBuy(def_shop.ShopToUse, true);
            }
            else if (shop is IListShop l_shop)
            {
                ShopHelper.ChangeToBuy(l_shop, true);
            }
        }

        private static void FillInData(IShopDescriptor shop)
        {
            Control.LogDebug(DInfo.ShopInterface, $"- fill data");
            if (shop == null)
                return;

            ShopHelper.FillInWidget(shop);
        }

        public static void ForceRefresh()
        {
            SwitchAndInit(ActiveShop);
        }

    }
}

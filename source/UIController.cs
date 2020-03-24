using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using BattleTech.UI;
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
            Control.LogDebug("Enter Shop Screen");
            if (shop_screen != ShopScreen)
                SetupButtons(shop_screen);

            StoreButton active = null;
            Control.LogDebug("- SetupButtons");
            foreach (var button in Buttons.Values)
            {
                var shop = button.Shop;
                Control.LogDebug($"-- [{shop.Name}]");
                Control.LogDebug($"--- Exists:{shop.Exists} CanUse:{shop.CanUse}");

                if (shop.Exists)
                {
                    if (shop is ISpriteIcon spr_icon)
                        button.Button.SetImageAndText(spr_icon.Sprite, shop.Name);
                    else if (shop is ITextIcon txt_icon)
                        Control.State.Sim.RequestItem<Sprite>(
                            txt_icon.SpriteID,
                            delegate (Sprite sprite) { button.Button.SetImageAndText(sprite, shop.Name); },
                            BattleTechResourceType.Sprite
                            );

                    button.Icon.color = shop.IconColor;

                    button.ButtonHolder.SetActive(true);
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

        private static void SetupButtons(SG_Shop_Screen shop_screen)
        {
            try
            {
                Control.LogDebug("- First Time enter. Start to setup");
                ShopScreen = shop_screen;
                ShopHelper = new ShopScreenHelper(ShopScreen);
                Buttons = new Dictionary<string, StoreButton>();


                var store_button = ShopHelper.SystemStoreButtonHoldingObject;
                var radio_set = ShopHelper.SystemStoreButtonHoldingObject.transform.parent.GetComponent<HBSRadioSet>();

                Control.LogDebug("-- Create buttons");
                foreach (var shop in Control.Shops)
                {

                    Control.LogDebug($"--- {shop.Name}");
                    var button = new StoreButton(store_button, shop);
                    Buttons.Add(shop.Name, button);
                }
                Control.LogDebug("-- Get default icons");
                Control.State.SystemShopSprite = ShopHelper.SystemStoreButtonHoldingObject.transform.GetChild(0)
                    .Find("tab_icon").GetComponent<Image>().sprite;
                ShopHelper.SimGame.RequestItem<Sprite>(SG_Shop_Screen.BLACKMARKET_ICON,
                    (sprite) => Control.State.BlacMarketSprite = sprite,
                    BattleTechResourceType.Sprite);


                Control.LogDebug("-- Hide original buttons");
                ShopHelper.SystemStoreButtonHoldingObject.SetActive(false);
                ShopHelper.FactionStoreButtonHoldingObject.SetActive(false);
                ShopHelper.BlackMarketStoreButtonHoldingObject.SetActive(false);

                Control.LogDebug("-- Setup radio set");
                radio_set.ClearRadioButtons();
                foreach (var pair in Buttons)
                    radio_set.AddButtonToRadioSet(pair.Value.Button);
                radio_set.defaultButton = Buttons.Values.First().Button;
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
            Control.LogDebug("-- done!");

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
            Control.LogDebug($"Pressed {shop.Name}");
            ActiveShop = shop;

            try
            {
                SwitchAndInit(shop);
                RefreshColors(shop);
                UpdateHeader(shop);
                FillInData(shop);
            }
            catch (Exception e)
            {
                Control.LogError("Error while switching shop", e);
            }
        }

        private static void SwitchAndInit(IShopDescriptor shop)
        {
            Control.LogDebug($"- fill shop");
            if (shop == null)
                return;

            if (shop is IDefaultShop def_shop)
            {
                Control.LogDebug($"-- IDefaultShop");
                ShopScreen.ChangeToBuy(def_shop.ShopToUse, true);
            }
            else
            {
                //TODO: ICustomShop
            }
        }

        private static void FillInData(IShopDescriptor shop)
        {
            Control.LogDebug($"- fill data");
            if (shop == null)
                return;

            if (shop is IFillWidgetFromFaction fill_shop)
            {
                Control.LogDebug($"-- IFillWidgetFromFaction");
                ShopHelper.FillInWithFaction(shop);
            }

        }

        private static void UpdateHeader(IShopDescriptor shop)
        {
            Control.LogDebug($"- update header");
            if (shop == null)
                return;

            ShopHelper.StoreHeaderText.SetText(shop.HeaderText);

            if (shop is ISpriteIcon spr_icon)
                ShopHelper.SetHeaderImageSpriteBySprite(spr_icon.Sprite);
            else if (shop is ITextIcon txt_icon)
                Control.State.Sim.RequestItem<Sprite>(
                    txt_icon.SpriteID,
                    delegate (Sprite sprite) { ShopHelper.SetHeaderImageSpriteBySprite(sprite); },
                    BattleTechResourceType.Sprite
                    );

            
            ShopHelper.StoreHeaderImageColor.OverrideWithColor(shop.IconColor);
        }
    }
}

using System;
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
                    button.Button.SetImageAndText(shop.Sprite, shop.Name);

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
                TabSelected(ActiveShop);
            }
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

        public static void RefreshColors()
        { 
            
        }

        internal static void TabSelected(IShopDescriptor shop)
        {
            Control.LogDebug($"Pressed {shop.Name}");
            ActiveShop = shop;
            ShopScreen.ChangeToBuy(shop.ShopToUse, true);
            RefreshColors();
        }
    }
}

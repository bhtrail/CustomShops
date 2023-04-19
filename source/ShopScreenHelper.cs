using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CustomShops
{
    public class ShopScreenHelper
    {
        public delegate void SetSpriteDelegate(Sprite sprite);

        public SG_Shop_Screen Screen { get; private set; }
        public  MiniFactionPanelHelper MiniWidgetHelper { get; private set; }

        public GameObject SystemStoreButtonHoldingObject { get; private set; }
        public GameObject BlackMarketStoreButtonHoldingObject { get; private set; }
        public GameObject FactionStoreButtonHoldingObject { get; private set; }
        public HBSDOTweenStoreTypeToggle SystemStoreButton { get; private set; }
        public SimGameState SimGame { get; private set; }
        public List<UIColorRefTracker> ColorAffectors { get; private set; }
        public UIColorRefTracker LargeBGFillColor { get; private set; }

        private LocalizableText CurrSystemText;
        private Image StoreImage;
        private SG_Stores_MiniFactionWidget miniFactionWidget;
        private HBSTooltip PlanetToolitp;

        public HBSDOTweenButton BuyButton { get; private set; }
        public HBSDOTweenToggle BuyTabButton { get; private set; }
        public HBSDOTweenToggle SellTabButton { get; private set; }
        public MechLabInventoryWidget_ListView inventoryWidget { get; private set; }

        public bool isInBuyingState
        {
            get => Screen.isInBuyingState;
            set => Screen.isInBuyingState = value;
        }
        public InventoryDataObject_SHOP selectedController
        {
            get => Screen.selectedController;
            set => Screen.selectedController = value;
        }
        public bool canPlayVO
        {
            get => Screen.canPlayVO;
            set => Screen.canPlayVO = value;
        }
        public bool triggerIronManAutoSave
        {
            get => Screen.triggerIronManAutoSave;
            set => Screen.triggerIronManAutoSave = value;
        }

        public ShopScreenHelper(SG_Shop_Screen screen)
        {
            Screen = screen;

            SystemStoreButtonHoldingObject = Screen.SystemStoreButtonHoldingObject;
            BlackMarketStoreButtonHoldingObject = Screen.BlackMarketStoreButtonHoldingObject;
            FactionStoreButtonHoldingObject = Screen.FactionStoreButtonHoldingObject;
            SystemStoreButton = Screen.SystemStoreButton;
            SimGame = Screen.simState;
            ColorAffectors = Screen.ColorAffectors;
            LargeBGFillColor = Screen.LargeBGFillColor;
            miniFactionWidget = Screen.StoreImagePanel.miniFactionWidget;
            MiniWidgetHelper = new MiniFactionPanelHelper(miniFactionWidget);
            CurrSystemText = Screen.StoreImagePanel.CurrSystemText;
            StoreImage = Screen.StoreImagePanel.StoreImage;
            PlanetToolitp = Screen.StoreImagePanel.PlanetToolitp;
            isInBuyingState = Screen.isInBuyingState;
            selectedController = Screen.selectedController;
            canPlayVO = Screen.canPlayVO;
            triggerIronManAutoSave = Screen.triggerIronManAutoSave;
            BuyButton = Screen.BuyButton;
            SellTabButton = Screen.SellTabButton;
            BuyTabButton = Screen.BuyTabButton;
            inventoryWidget = Screen.inventoryWidget;
        }

        public void FillInWidget(IShopDescriptor shop)
        {
            try
            {
                Control.LogDebug(DInfo.ShopInterface, $"-- FillInWidget");

                string id = shop.ShopPanelImage;
                Control.LogDebug(DInfo.ShopInterface, $"--- request panel image id:{id}");
                Control.State.Sim.RequestItem<Sprite>(id, sprite =>
                {
                    Control.LogDebug(DInfo.ShopInterface, $"Received panel image");
                    StoreImage.sprite = sprite;
                    Control.LogDebug(DInfo.ShopInterface, $"- set");
                }, BattleTechResourceType.Sprite);

                MiniWidgetHelper.ReputationBonusText.gameObject.SetActive(true);
                if (shop is IFillWidgetFromFaction f_fill)
                {
                    var faction = f_fill.RelatedFaction;
                    if (faction == null)
                        faction = FactionEnumeration.GetInvalidUnsetFactionValue();
                    miniFactionWidget.FillInData(faction);
                }
                else if (shop is ICustomFillWidget c_fill)
                {
                    c_fill.FillFactionWidget(this);
                }


                if (shop is ICustomDiscount c_discount)
                {
                    MiniWidgetHelper.ReputationBonusText.gameObject.SetActive(true);
                    var discount = c_discount.GetDiscount(null) - 1;

                    var str = "Reputation: ";
                    if (discount > 0f)
                        str += $"{Mathf.Abs(Mathf.RoundToInt(discount * 100f))}% Price Increase";
                    else if (discount < 0f)
                        str += $"{Mathf.Abs(Mathf.RoundToInt(-discount * 100f))}% Discount";
                    else
                        str += "At Cost";

                    MiniWidgetHelper.ReputationBonusText.SetText(str);
                }

                CurrSystemText.SetText(Control.State.CurrentSystem.Name);
                if (PlanetToolitp != null)
                    PlanetToolitp.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(Control.State.CurrentSystem));
            }
            catch (Exception e)
            {
                Control.LogError(e);
            }
        }

        public void FillCusotm(IShopDescriptor shop)
        {
            var f_custom = shop as ICustomFillWidget;
            f_custom.FillFactionWidget(this);
        }

        internal void ChangeToBuy(IListShop l_shop, bool v)
        {
            bool tort(ShopDefItem item, out BattleTechResourceType result)
            {
                switch (item.Type)
                {
                    case ShopItemType.Weapon:
                        result = BattleTechResourceType.WeaponDef;
                        break;
                    case ShopItemType.AmmunitionBox:
                        result = BattleTechResourceType.AmmunitionBoxDef;
                        break;
                    case ShopItemType.HeatSink:
                        result = BattleTechResourceType.HeatSinkDef;
                        break;
                    case ShopItemType.JumpJet:
                        result = BattleTechResourceType.JumpJetDef;
                        break;
                    case ShopItemType.Mech:
                    case ShopItemType.MechPart:
                        result = BattleTechResourceType.MechDef;
                        break;
                    case ShopItemType.Upgrade:
                        result = BattleTechResourceType.UpgradeDef;
                        break;
                    default:
                        result = BattleTechResourceType.AssetBundle;
                        return false;
                }

                return true;
            }

            var shop = new Shop(Control.State.Sim, Control.State.CurrentSystem, null, Shop.RefreshType.None, Shop.ShopType.System);
                shop.ActiveInventory.Clear();
                shop.ActiveInventory.AddRange(l_shop.Items.Where(i => tort(i, out var t) && Control.State.Sim.DataManager.Exists(t, i.ID)));
                Screen.ChangeToBuy(shop, true);
        }
    }
}
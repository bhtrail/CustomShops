using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;
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

        public Traverse Main { get; private set; }
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

        private Traverse<bool> T_isInBuyingState;
        public bool isInBuyingState
        {
            get => T_isInBuyingState.Value;
            set => T_isInBuyingState.Value = value;
        }
        private Traverse<InventoryDataObject_SHOP> T_selectedController;
        public InventoryDataObject_SHOP selectedController
        {
            get => T_selectedController.Value;
            set => T_selectedController.Value = value;
        }
        private Traverse<bool> T_canPlayVO;
        public bool canPlayVO
        {
            get => T_canPlayVO.Value;
            set => T_canPlayVO.Value = value;
        }
        private Traverse<bool> T_triggerIronManAutoSave;
        public bool triggerIronManAutoSave
        {
            get => T_triggerIronManAutoSave.Value;
            set => T_triggerIronManAutoSave.Value = value;
        }


        public ShopScreenHelper(SG_Shop_Screen screen)
        {
            Screen = screen;
            Main = new Traverse(Screen);

            SystemStoreButtonHoldingObject = Main.Field<GameObject>("SystemStoreButtonHoldingObject").Value;
            BlackMarketStoreButtonHoldingObject = Main.Field<GameObject>("BlackMarketStoreButtonHoldingObject").Value;
            FactionStoreButtonHoldingObject = Main.Field<GameObject>("FactionStoreButtonHoldingObject").Value;
            SystemStoreButton = Main.Field<HBSDOTweenStoreTypeToggle>("SystemStoreButton").Value;
            SimGame = Main.Field<SimGameState>("simState").Value;
            ColorAffectors = Main.Field<List<UIColorRefTracker>>("ColorAffectors").Value;
            LargeBGFillColor = Main.Field<UIColorRefTracker>("LargeBGFillColor").Value;
            var StoreImagePanel = Main.Field("StoreImagePanel");
            miniFactionWidget = StoreImagePanel.Field<SG_Stores_MiniFactionWidget>("miniFactionWidget").Value;
            MiniWidgetHelper = new MiniFactionPanelHelper(miniFactionWidget);
            CurrSystemText = StoreImagePanel.Field<LocalizableText>("CurrSystemText").Value;
            StoreImage = StoreImagePanel.Field<Image>("StoreImage").Value;
            PlanetToolitp = StoreImagePanel.Field<HBSTooltip>("PlanetToolitp").Value;
            T_isInBuyingState = Main.Field<bool>("isInBuyingState");

            T_selectedController = Main.Field<InventoryDataObject_SHOP>("selectedController");
            T_canPlayVO = Main.Field<bool>("canPlayVO");
            T_triggerIronManAutoSave = Main.Field<bool>("triggerIronManAutoSave");

            BuyButton = Main.Field<HBSDOTweenButton>("BuyButton").Value;
            SellTabButton = Main.Field<HBSDOTweenToggle>("SellTabButton").Value;
            BuyTabButton = Main.Field<HBSDOTweenToggle>("BuyTabButton").Value;
            inventoryWidget = Main.Field<MechLabInventoryWidget_ListView>("inventoryWidget").Value;
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
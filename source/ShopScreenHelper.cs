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

        //public Traverse Main { get; private set; }
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

        //private Traverse<bool> T_isInBuyingState;
        public bool isInBuyingState
        {
            get => Screen.isInBuyingState;//T_isInBuyingState.Value);
            set => Screen.isInBuyingState = value; //T_isInBuyingState.Value = value;
        }
        //private Traverse<InventoryDataObject_SHOP> T_selectedController;
        public InventoryDataObject_SHOP selectedController
        {
            get => Screen.selectedController; //T_selectedController.Value;
            set => Screen.selectedController = value; //T_selectedController.Value = value;
        }
        //private Traverse<bool> T_canPlayVO;
        public bool canPlayVO
        {
            get => Screen.canPlayVO; //T_canPlayVO.Value;
            set => Screen.canPlayVO = value; //T_canPlayVO.Value = value;
        }
        //private Traverse<bool> T_triggerIronManAutoSave;
        public bool triggerIronManAutoSave
        {
            get => Screen.triggerIronManAutoSave; //T_triggerIronManAutoSave.Value;
            set => Screen.triggerIronManAutoSave = value; //T_triggerIronManAutoSave.Value = value;
        }

        public ShopScreenHelper(SG_Shop_Screen screen)
        {
            Screen = screen;
            //Main = new Traverse(Screen);

            //SystemStoreButtonHoldingObject = Main.Field<GameObject>("SystemStoreButtonHoldingObject").Value;
            SystemStoreButtonHoldingObject = Screen.SystemStoreButtonHoldingObject;
            //BlackMarketStoreButtonHoldingObject = Main.Field<GameObject>("BlackMarketStoreButtonHoldingObject").Value;
            BlackMarketStoreButtonHoldingObject = Screen.BlackMarketStoreButtonHoldingObject;
            //FactionStoreButtonHoldingObject = Main.Field<GameObject>("FactionStoreButtonHoldingObject").Value;
            FactionStoreButtonHoldingObject = Screen.FactionStoreButtonHoldingObject;
            //SystemStoreButton = Main.Field<HBSDOTweenStoreTypeToggle>("SystemStoreButton").Value;
            SystemStoreButton = Screen.SystemStoreButton;
            //SimGame = Main.Field<SimGameState>("simState").Value;
            SimGame = Screen.simState;
            //ColorAffectors = Main.Field<List<UIColorRefTracker>>("ColorAffectors").Value;
            ColorAffectors = Screen.ColorAffectors;
            //LargeBGFillColor = Main.Field<UIColorRefTracker>("LargeBGFillColor").Value;
            LargeBGFillColor = Screen.LargeBGFillColor;
            //var StoreImagePanel = Main.Field("StoreImagePanel");
            
            //miniFactionWidget = StoreImagePanel.Field<SG_Stores_MiniFactionWidget>("miniFactionWidget").Value;
            miniFactionWidget = Screen.StoreImagePanel.miniFactionWidget;
            MiniWidgetHelper = new MiniFactionPanelHelper(miniFactionWidget);

            //CurrSystemText = StoreImagePanel.Field<LocalizableText>("CurrSystemText").Value;
            CurrSystemText = Screen.StoreImagePanel.CurrSystemText;
            //StoreImage = StoreImagePanel.Field<Image>("StoreImage").Value;
            StoreImage = Screen.StoreImagePanel.StoreImage;
            //PlanetToolitp = StoreImagePanel.Field<HBSTooltip>("PlanetToolitp").Value;
            PlanetToolitp = Screen.StoreImagePanel.PlanetToolitp;
            //T_isInBuyingState = Main.Field<bool>("isInBuyingState");
            isInBuyingState = Screen.isInBuyingState;
            //T_selectedController = Main.Field<InventoryDataObject_SHOP>("selectedController");
            selectedController = Screen.selectedController;
            //T_canPlayVO = Main.Field<bool>("canPlayVO");
            canPlayVO = Screen.canPlayVO;
            //T_triggerIronManAutoSave = Main.Field<bool>("triggerIronManAutoSave");
            triggerIronManAutoSave = Screen.triggerIronManAutoSave;
            //BuyButton = Main.Field<HBSDOTweenButton>("BuyButton").Value;
            BuyButton = Screen.BuyButton;
            //SellTabButton = Main.Field<HBSDOTweenToggle>("SellTabButton").Value;
            SellTabButton = Screen.SellTabButton;
            //BuyTabButton = Main.Field<HBSDOTweenToggle>("BuyTabButton").Value;
            BuyTabButton = Screen.BuyTabButton;
            //inventoryWidget = Main.Field<MechLabInventoryWidget_ListView>("inventoryWidget").Value;
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
using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BattleTech.UI.Tooltips;
using Harmony;
using System.Collections.Generic;
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

        public GameObject SystemStoreButtonHoldingObject { get; private set; }
        public GameObject BlackMarketStoreButtonHoldingObject { get; private set; }
        public GameObject FactionStoreButtonHoldingObject { get; private set; }
        public HBSDOTweenStoreTypeToggle SystemStoreButton { get; private set; }
        public SimGameState SimGame { get; private set; }
        public List<UIColorRefTracker> ColorAffectors { get; private set; }
        public UIColorRefTracker LargeBGFillColor { get; private set; }

        public LocalizableText StoreHeaderText { get; private set; }
        public UIColorRefTracker StoreHeaderImageColor { get; private set; }

        private MethodInfo setHeaderImageSpriteBySprite;
        private LocalizableText CurrSystemText;
        private Image StoreImage;
        private SG_Stores_MiniFactionWidget miniFactionWidget;
        private HBSTooltip PlanetToolitp;

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

            StoreHeaderText = Main.Field<LocalizableText>("StoreHeaderText").Value;
            StoreHeaderImageColor = Main.Field<UIColorRefTracker>("StoreHeaderImageColor").Value;
            setHeaderImageSpriteBySprite = AccessTools.Method(typeof(SG_Shop_Screen), "SetHeaderImageSpriteBySprite");

            var StoreImagePanel = Main.Field("StoreImagePanel");
            miniFactionWidget = StoreImagePanel.Field<SG_Stores_MiniFactionWidget>("miniFactionWidget").Value;
            CurrSystemText = StoreImagePanel.Field<LocalizableText>("CurrSystemText").Value;
            StoreImage = StoreImagePanel.Field<Image>("StoreImage").Value;
            PlanetToolitp = StoreImagePanel.Field<HBSTooltip>("PlanetToolitp").Value;
        }

        public void SetHeaderImageSpriteBySprite(Sprite sprite)
        {
            setHeaderImageSpriteBySprite.Invoke(Screen, new object[] { sprite });
        }

        public void FillInWithFaction(IShopDescriptor shop)
        {
            var faction = (shop as IFillWidgetFromFaction).RelatedFaction;
            if (faction == null)
                faction = FactionEnumeration.GetInvalidUnsetFactionValue();

            Control.State.Sim.RequestItem<Sprite>(shop.ShopPanelImage, sprite => StoreImage.sprite = sprite, BattleTechResourceType.Sprite);
            miniFactionWidget.FillInData(faction);
            CurrSystemText.SetText(Control.State.CurrentSystem.Name);
            if (PlanetToolitp != null)
                PlanetToolitp.SetDefaultStateData(TooltipUtilities.GetStateDataFromObject(Control.State.CurrentSystem));
        }
    }
}
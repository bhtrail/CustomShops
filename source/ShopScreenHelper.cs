using BattleTech;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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
            SG_Stores_MiniFactionWidget miniFactionWidget = StoreImagePanel.Field<SG_Stores_MiniFactionWidget>("miniFactionWidget").Value;
        }

        public void SetHeaderImageSpriteBySprite(Sprite sprite)
        {
            setHeaderImageSpriteBySprite.Invoke(Screen, new object[] { sprite });
        }

        public void FillInWithFaction(FactionValue faction, IShopDescriptor shop)
        {
            if (faction == null)
                faction = FactionEnumeration.GetInvalidUnsetFactionValue();


        }
    }
}
using BattleTech;
using BattleTech.UI;
using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace CustomShops
{
    public class ShopScreenHelper
    {
        public Traverse Main { get; private set; }
        public SG_Shop_Screen Screen { get; private set; }

        public GameObject SystemStoreButtonHoldingObject { get; private set; }
        public GameObject BlackMarketStoreButtonHoldingObject { get; private set; }
        public GameObject FactionStoreButtonHoldingObject { get; private set; }
        public HBSDOTweenStoreTypeToggle SystemStoreButton { get; private set; }
        public SimGameState SimGame { get; private set; }
        public List<UIColorRefTracker> ColorAffectors { get; private set; }

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
        }
    }
}
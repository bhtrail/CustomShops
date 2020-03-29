using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using HBS.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomShops
{
    public class TSMTagInfo
    {
        public string Tag { get; set; }
        public float Mul { get; set; }
    }

    [Flags]
    public enum DInfo 
    {
        NONE = 0,
        ShopInterface = 1,
        ShowItemList = 2,
        DetailDebug = 4,
        SaveLoad = 8,
        RefreshShop = 16,
        TabSwitch = 32,
        ShopActions = 64,
        TypedItemDef = 128,
        BuyBack = 256,
        ALL = 0xffff
    }

    public class CustomShopsSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;
        public bool AddLogPrefix = false;
        public DInfo DebugType= DInfo.ShopInterface | DInfo.SaveLoad | DInfo.RefreshShop | DInfo.TabSwitch | DInfo.ShopActions | DInfo.BuyBack | DInfo.TypedItemDef;
        public bool DEBUG_FactionShopAlwaysAvaliable = false;
        public bool DEBUG_BlackMarketAlwaysAvaliable = false;



        public bool SystemShop  = true;
        public bool FactionShop  = true;
        public bool BlackMarketShop  = true;
        public bool BuyBackShop = true;

        public bool AllowMultiSell = true;
        public bool AllowMultiBuy = true;

        public bool ShowConfirm = true;
        public int ConfirmLowLimit = 100000;
        public float FactionShopAdjustment = -0.25f;


    }
}

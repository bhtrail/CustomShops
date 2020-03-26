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
        ALL = 0xffff
    }

    public class CustomShopsSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;
        public bool AddLogPrefix = false;
        public DInfo DebugType { get; set; } = DInfo.ALL;

        public bool SystemShop { get; set; } = true;
        public bool FactionShop { get; set; } = true;
        public bool BlackMarketShop { get; set; } = true;
    }
}

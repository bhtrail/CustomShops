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

    public class CustomShopsSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;
        public bool AddLogPrefix = false;

        public bool SystemShop { get; set; } = true;
        public bool FactionShop { get; set; } = true;
        public bool BlackMarketShop { get; set; } = true;
    }
}

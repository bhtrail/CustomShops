using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomShops
{
    public interface IShopDescriptor
    {
        string Name { get; }
        string TabText { get; }
        string HeaderText { get; }
        string ShopPanelImage { get; }
        public Color IconColor { get; }
        public Color ShopColor { get; }
        public bool Exists { get; }
        public bool CanUse { get; }

        public bool RefreshOnSystemChange { get; }
        public bool RefreshOnMonthChange { get; }
        public bool RefreshOnOwnerChange { get; }
        void RefreshShop();
    }
}

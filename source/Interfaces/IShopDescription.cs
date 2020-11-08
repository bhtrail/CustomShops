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
        string ShopPanelImage { get; }
        public Color IconColor { get; }
        public Color ShopColor { get; }
        public bool Exists { get; }
        public bool CanUse { get; }
        public int SortOrder { get; }

        [Obsolete]
        public bool RefreshOnSystemChange { get; }
        [Obsolete]
        public bool RefreshOnMonthChange { get; }
        [Obsolete]
        public bool RefreshOnOwnerChange { get; }
        void RefreshShop();
    }

    public interface IListShop
    {
        List<ShopDefItem> Items { get; }
    }

    public interface ICustomPurshase
    {
        bool Purshase(ShopDefItem item, int quantity);
    }
}
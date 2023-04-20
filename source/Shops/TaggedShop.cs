using System;
using System.Collections.Generic;
using BattleTech;
using UnityEngine;

namespace CustomShops
{
    public abstract class TaggedShop : IShopDescriptor, IDefaultShop, ISaveShop
    {
        public abstract string Name { get; }
        public abstract string TabText { get; }
        public abstract string ShopPanelImage { get; }
        public abstract Color IconColor { get; }
        public abstract Color ShopColor { get; }

        public abstract bool Exists { get; }
        public abstract bool CanUse { get; }

        public List<string> Tags { get; set; }
        public Shop Shop { get; set; }
        //public Traverse ShopT;

        public Shop ShopToUse => Shop;

        public abstract int SortOrder { get; }
        public abstract bool RefreshOnSystemChange { get; }
        public abstract bool RefreshOnMonthChange { get; }
        public abstract bool RefreshOnOwnerChange { get; }

        protected abstract void UpdateTags();

        private void Initilize()
        {
            if (Shop == null)
            {
                Shop = new Shop();
            }

#if CCDEBUG
            if (Tags != null)
                foreach (var item in Tags)
                {
                    Control.LogDebug(DInfo.RefreshShop, "-- " + item);
                }
            else
                Control.LogDebug(DInfo.RefreshShop, "-- Empty");
#endif
            Shop.Sim = UnityGameInstance.BattleTechGame.Simulation;
            Shop.system = Control.State.CurrentSystem;
            if (Shop.ItemCollections == null)
                Shop.ItemCollections = new List<ItemCollectionDef>();
            else
                Shop.ItemCollections.Clear();

            if (Tags != null)
                foreach (var item in Tags)
                {
                    try
                    {
                        var col = Control.State.Sim.DataManager.ItemCollectionDefs.Get(item);
                        if (col == null)
                        {
                            Control.LogError("Cannot retrive ItemCollection " + item + ", skipping");
                            continue;
                        }
                        Shop.ItemCollections.Add(col);
                    }
                    catch (Exception e)
                    {
                        Control.LogError("Cannot retrive ItemCollection " + item + ", skipping", e);
                    }
                }
        }

        public void RefreshShop()
        {
            UpdateTags();
            Initilize();
            Shop.RefreshShop();
        }

        public virtual Shop GetShopToSave() { return Shop; }
        public virtual void SetLoadedShop(Shop shop)
        {
            Shop = shop;
            if (shop != null)
            {
                Shop.Rehydrate(Control.State.Sim, Control.State.CurrentSystem, Tags, Shop.RefreshType.None, Shop.ShopType.System);
            }
            else
                Initilize();
        }
    }
}

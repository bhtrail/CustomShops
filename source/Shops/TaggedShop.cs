using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleTech;
using UnityEngine;
using Harmony;

namespace CustomShops
{
    public abstract class TaggedShop : IShopDescriptor
    {
        public abstract string Name { get; }
        public abstract Sprite Sprite { get; }
        public abstract Color IconColor { get; }
        public abstract Color ShopColor { get; }

        public abstract bool Exists { get; }
        public abstract bool CanUse { get; }

        public List<string> Tags { get; set; }
        public Shop Shop { get; set; }
        public Traverse ShopT;


        public abstract bool RefreshOnSystemChange { get; }
        public abstract bool RefreshOnMonthChange { get; }
        public abstract bool RefreshOnOwnerChange { get; }
        public abstract bool RefreshOnGameLoad { get; }
        public abstract bool NeedSave { get; }


        public virtual void Initilize()
        {
            if (Shop == null)
            {
                Shop = new Shop();
                ShopT = new Traverse(Shop);
            }

#if CCDEBUG
            if (Tags != null)
                foreach (var item in Tags)
                {
                    Control.LogDebug("-- " + item);
                }
            else
                Control.LogDebug("-- Empty");
#endif
            ShopT.Field<SimGameState>("Sim").Value = UnityGameInstance.BattleTechGame.Simulation;
            ShopT.Field<StarSystem>("system").Value = Control.State.CurrentSystem;

            Shop.Initialize(Tags, Shop.ShopType.System);
        }


        public void RefreshShop()
        {
            Initilize();
            Shop.RefreshShop();
        }

        public virtual Shop GetShopToSave()
        {
            return Shop;
        }

        public abstract void SetLoadedShop(Shop shop);
    }
}

using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShops
{ 
    public interface IDefaultPrice
    {
    }

    public interface ICustomPrice
    {
        public int GetPrice(TypedShopDefItem item);
    }

    public interface IDiscountFromFaction : IRelatedFaction
    {
    }

    public interface ICustomDiscount
    {
        public float GetDiscount();
    }

    public interface INoDiscount
    {
    }
}

using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShops
{
    public interface IDefaultShop
    {
        Shop ShopToUse { get; }
    }
}

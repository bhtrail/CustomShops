using BattleTech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShops
{
    public interface ISaveShop
    {
        Shop GetShopToSave();
        void SetLoadedShop(Shop shop);
    }
}

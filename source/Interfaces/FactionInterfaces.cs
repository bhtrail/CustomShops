using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomShops
{
    public interface IRelatedFaction
    {
        BattleTech.FactionValue RelatedFaction { get; }
    }

    public interface IFillWidgetFromFaction : IRelatedFaction
    {
    }

    public interface ICustomFillWidget
    {
        void FillFactionWidget(ShopScreenHelper helper);
    }
}

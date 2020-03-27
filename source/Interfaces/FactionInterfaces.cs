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

    public interface IDiscountFromFaction : IRelatedFaction
    {
    }

    public interface ICustomDiscount
    {
    }

    public interface INoDiscount
    { 
    }

    public interface ICustomFillWidget
    {
        void FillFactionWidget(MiniFactionPanelHelper helper);
    }
}

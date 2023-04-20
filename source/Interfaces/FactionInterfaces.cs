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

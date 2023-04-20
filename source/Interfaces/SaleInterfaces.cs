using BattleTech;

namespace CustomShops
{
    public interface ISellShop
    {
        public int SellPriority { get; }
        public bool OnSellItem(ShopDefItem item, int num);
    }
}

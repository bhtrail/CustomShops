using BattleTech;

namespace CustomShops
{
    public interface ISaveShop
    {
        Shop GetShopToSave();
        void SetLoadedShop(Shop shop);
    }
}

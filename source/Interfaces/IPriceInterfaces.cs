namespace CustomShops;

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
    public float GetDiscount(TypedShopDefItem item);
}

public interface INoDiscount
{
}

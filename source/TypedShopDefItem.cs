using BattleTech;
using System;

namespace CustomShops;

public class TypedShopDefItem : ShopDefItem
{
    public DescriptionDef Description { get; private set; }
    public MechComponentDef Component { get; private set; }
    public MechDef Mech { get; private set; }

    public TypedShopDefItem(ShopDefItem item)
    {
        try
        {
            Control.LogDebug(DInfo.TypedItemDef, "TypedShopDefItem.ctor");
            Count = item.Count;
            ID = item.ID;
            Type = item.Type;
            DiscountModifier = item.DiscountModifier;
            Count = item.Count;
            IsInfinite = item.IsInfinite;
            IsDamaged = item.IsDamaged;
            SellCost = item.SellCost;
            SetGuid(item.GUID);
            Control.LogDebug(DInfo.TypedItemDef, "data copied");

            var dm = Control.State.Sim.DataManager;

            switch (item.Type)
            {
                case ShopItemType.Weapon:
                    Control.LogDebug(DInfo.TypedItemDef, "Weapon");
                    WeaponDef weaponDef = null;
                    if (dm.WeaponDefs.Exists(GUID))
                    {
                        weaponDef = dm.WeaponDefs.Get(GUID);
                    }

                    Component = weaponDef;
                    Description = weaponDef.Description;
                    break;
                case ShopItemType.AmmunitionBox:
                    Control.LogDebug(DInfo.TypedItemDef, "AmmunitionBox");
                    AmmunitionBoxDef ammoBoxDef = null;
                    if (dm.AmmoBoxDefs.Exists(GUID))
                    {
                        ammoBoxDef = dm.AmmoBoxDefs.Get(GUID);
                    }

                    Component = ammoBoxDef;
                    Description = ammoBoxDef.Description;
                    break;
                case ShopItemType.HeatSink:
                    Control.LogDebug(DInfo.TypedItemDef, "HeatSink");
                    HeatSinkDef hsDef = null;
                    if (dm.HeatSinkDefs.Exists(GUID))
                    {
                        hsDef = dm.HeatSinkDefs.Get(GUID);
                    }

                    Component = hsDef;
                    Description = hsDef.Description;
                    break;
                case ShopItemType.JumpJet:
                    Control.LogDebug(DInfo.TypedItemDef, "JumpJet");
                    JumpJetDef jjDef = null;
                    if (dm.JumpJetDefs.Exists(GUID))
                    {
                        jjDef = dm.JumpJetDefs.Get(GUID);
                    }

                    Component = jjDef;
                    Description = jjDef.Description;
                    break;
                case ShopItemType.Upgrade:
                    Control.LogDebug(DInfo.TypedItemDef, "Upgrade");
                    UpgradeDef upgradeDef = null;
                    if (dm.UpgradeDefs.Exists(GUID))
                    {
                        upgradeDef = dm.UpgradeDefs.Get(GUID);
                    }

                    Component = upgradeDef;
                    Description = upgradeDef.Description;
                    break;

                case ShopItemType.MechPart:
                case ShopItemType.Mech:
                    Control.LogDebug(DInfo.TypedItemDef, item.Type.ToString());

                    var id = Control.GetMDefFromCDef(GUID);
                       

                    Control.LogDebug(DInfo.TypedItemDef, $"MechPart/Mech {GUID} {ID} {id}");

                    if (dm.MechDefs.Exists(id))
                    {
                        Mech = dm.MechDefs.Get(id);
                    }
                    else
                    {
                        Control.LogError($"Cannot find Mech for {id}");
                    }

                    Description = Type == ShopItemType.MechPart ? Mech.Description : Mech.Chassis.Description;
                    break;
            }
            Control.LogDebug(DInfo.TypedItemDef, "done");

        }
        catch (Exception e)
        {
            Control.LogError(e);
        }
    }
}

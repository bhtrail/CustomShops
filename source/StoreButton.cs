using BattleTech.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CustomShops;

public class StoreButton
{
    public GameObject ButtonHolder;
    public HBSDOTweenStoreTypeToggle Button;
    public IShopDescriptor Shop;
    public GameObject Overlay;
    public Image Icon;

    public StoreButton(GameObject store_button, IShopDescriptor shop)
    {
        Shop = shop;
        ButtonHolder = Object.Instantiate(store_button);
        ButtonHolder.transform.SetParent(store_button.transform.parent);

        Button = ButtonHolder.transform.GetChild(0).GetComponent<HBSDOTweenStoreTypeToggle>();
        Overlay = ButtonHolder.transform.GetChild(1).gameObject;
        Button.OnClicked.RemoveAllListeners();
        Button.OnClicked.AddListener(OnClick);
        Icon = Button.transform.Find("tab_icon").GetComponent<Image>();
    }

    private void OnClick()
    {
        UIController.TabSelected(Shop);
    }
}
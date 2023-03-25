using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class ProductButton : MonoBehaviour
{
    [SerializeField] private string Name; //Ten trung voi ten co trong Name list product IAP
    //[SerializeField] private TextMeshProUGUI price;
    [SerializeField] string productID;
    [SerializeField] UIButton btnBuy;

    public bool IsInit = true;

    private void Awake()
    {
        if(IsInit) ReloadCost();
    }

    public void CallStart(string _Name, string _productID)
    {
        Name = _Name;
        productID = _productID;
        btnBuy.SetUpEvent(OnClick);
        ReloadCost();
    }

    private void OnEnable()
    {
        FirebaseManager.Instance?.Event_IAP_offer(productID, "Popup_Setting");
    }

    public bool UpdateData(string productName)
    {
        bool sucsses = false;
        if (PurchaserManager.Instance == null) return false;
        foreach (var item in PurchaserManager.Instance.productIAP)
        {
            if (item.Name == productName)
            {
                Name = productName;
                if (PurchaserManager.Instance.IsInitialized())
                {
                    //if (price != null)
                    //{
                    //    string priceStr = PurchaserManager.m_StoreController.products.WithID(item.ID).metadata.localizedPriceString.ToString();
                    //    if (priceStr != "")
                    //    {
                    //        price.text = priceStr;
                            sucsses = true;
                    //    }
                    //}
                }
                productID = item.ID;
                break;
            }
        }

#if !UNITY_EDITOR
        bool result = true;
        if (Application.internetReachability == NetworkReachability.NotReachable)
            result = false;
        if (!result)
        {
            //price.text = "4.99$";
        }
#endif
        return sucsses;
    }

    private void ReloadCost()
    {
        if (!UpdateData(Name))
            Invoke("ReloadCost", 1);
    }

    public void OnClick()
    {
        if (PurchaserManager.Instance != null)
        {
            if (productID != "")
            {
                if (productID == "removeads")
                {
                    FirebaseManager.Instance.Click_RemoveAds();
                    FirebaseManager.Instance?.Event_IAP_get(productID, "Popup_Setting");
                }
                PurchaserManager.Instance.BuyProductID(productID);
            }
        }
    }
}

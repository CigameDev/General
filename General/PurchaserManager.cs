using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class PurchaserManager : MonoBehaviour, IStoreListener
{
    public static PurchaserManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;
    public string AndroidKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAog0J2lr+KmfVg8LOwv1o/rHtg10Shd4Wvgimk6STKTmF/iiP4fJbH11hQDOODlMe6AczZrLnJnux6rCljEhC9oIcJvWfT0N+QsbROKn3cDOkcL6+hCiio3JTc0q5YPtCIM8w/jdGqUPP0kpRq2f/pp1mM5vgRnvmkjo7aRX/QADHt91RtL9ju1A4IteHH2xivDdnw9wsfzvh95xOo8eA0uCg48uKUxGgufFinXeF25A030okFzuCbj6x5qYbslGPHmZSH0ee1+bFGjxmTJpFw4aaBmWa8ru7OgfdY6Pty8a5xk4X0RxBMg8rPvm8QO/yjQ3vhA4goBnimUJGSt/oQQIDAQAB";
    public List<ProductIAP> productIAP;
    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //builder.Configure<IGooglePlayConfiguration>().SetPublicKey(AndroidKeyVIP);

        //KHOI TAO CAC TEN MAT HANG TREN CAC STORE
        foreach (var item in productIAP)
        {
            builder.AddProduct(item.ID, item.productType, new IDs(){
                {item.ID, AppleAppStore.Name.ToString()},//dinh nghia them cho ro rang cac store, cung khong can thiet + IOS
                {item.ID, GooglePlay.Name.ToString()},
                //{item.ID, FacebookStore.Name.ToString()},
                {item.ID, WindowsStore.Name.ToString()},
                {item.ID, AmazonApps.Name.ToString()},
                //{item.ID, TizenStore.Name.ToString()},
                //{item.ID, XiaomiMiPay.Name.ToString()},
                //{item.ID, MoolahAppStore.Name.ToString()},
                {item.ID, MacAppStore.Name.ToString()}
            });
        }
        UnityPurchasing.Initialize(this, builder);
    }


    public void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
                //FirebaseManager.Instance.LogEvent_trackIAP_CLICK(product.definition.storeSpecificId);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {

            Debug.Log("BuyProductID FAIL. Not initialized.");
            InitializePurchasing();
        }
    }

    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            RestoreIAP();
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    bool allowRetore = false;
    public void RestoreIAP()
    {
        if (!allowRetore) return;

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {

    }

    public void Cheat_IAP(string id)
    {

    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log(args.purchasedProduct.definition.id);
       
        foreach (var item in productIAP)
        {
            
            if (String.Equals(args.purchasedProduct.definition.id, item.ID, StringComparison.Ordinal))
            switch (item.Type)
            {
                    case ModificationType.RemoveAds:
                        FirebaseManager.Instance?.Event_IAP_purchase_success(item.ID,"Popup_Setting");
                        FirebaseManager.Instance.Click_RemoveAdsSuccess();
                        break;
                }
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        FirebaseManager.Instance?.Event_IAP_purchase_fail(product.definition.storeSpecificId, "Popup_Setting", failureReason.ToString());
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void AutoBuy(ProductType type, int quantity)
    {
        foreach (var item in productIAP)
        {
            if (item.productType == type && item.Count >= quantity)
            {
                BuyProductID(item.ID);
                break;
            }
        }
    }

    public void Logger(string log)
    {
        Debug.Log(log);
    }
}
[System.Serializable]
public class ProductIAP
{
    public string Name;
    public string ID;
    public ProductType productType;
    public ModificationType Type;
    public int Count;
}

public enum ModificationType
{
    RemoveAds
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsController : Singleton<AdsController>
{
    // Start is called before the first frame update
    private void Hack()
    {
        PlayerPrefs.SetInt("DeleteAds", 1);
        GameData.Data.CurrentLevelMax = 32;
    }

    private string demo;
    void Start()
    {
        //Hack();
        //return;
        timeNewSessionInter = DateTime.UtcNow.ToString();
        timeNewSessionReward = DateTime.UtcNow.ToString();
        timeNewSessionOA = DateTime.UtcNow.ToString();

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
            demo = DateTime.UtcNow.ToString();
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenAdRevenuePaidEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenAdDisplayFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenAdClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += (s, info) =>
            {
                FirebaseManager.Instance?.Ads_OpenRequest();
                DateTime tmp = Convert.ToDateTime(demo);
                TimeSpan TimeSesion = DateTime.UtcNow - tmp;
                double Session = TimeSesion.TotalSeconds;
                //Debug.LogError("Time: " + Session);
            };
        };

        MaxSdk.SetSdkKey(SDKKey);
        MaxSdk.InitializeSdk();

        InitializeRewardedAds();
        InitializeInterstitialAds();
        InitializeBannerAds();

    }

    public string SDKKey = "sdKdF4f4EDX3gDNhzOUhyJJb5ITQnA8hbqpGLC6P-70pG-kW7OuJyLP8BFqS1bC41L2hc1Dibn168yhzNbTD6T";
    public string IDReward = "51bcf2ab16a58c40";
    public string IDInter = "2caa91101bfcab8b";
    public string IDBanner = "967630aae3849e80";
    public string AppOpenAdUnitId = "d7c39fc404e69185";
    int retryAttemptReward;


    #region Reward

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(IDReward);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        FirebaseManager.Instance?.Ads_RewardRequest();
        retryAttemptReward = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttemptReward++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptReward));

        FirebaseManager.Instance?.Ads_RewardFailed();
        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        FirebaseManager.Instance?.Event_ads_reward_fail("skip", errorInfo.Message);
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad\
        doneReward?.Invoke();

        FirebaseManager.Instance?.Ads_RewardComplete(LevelControl.Instance.CurrentLevel);
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance?.Event_ads_reward_complete("skip");
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        FirebaseManager.Instance.Event_ad_impression(adUnitId, adInfo);
    }
    System.Action doneReward;
    private string timeNewSessionReward;

    public void ShowReward(System.Action complete, System.Action done, System.Action fail)
    {
        if (PlayerPrefs.GetInt("DeleteAds") == 1)
        {
            done?.Invoke();
            return;
        }
        FirebaseManager.Instance?.Event_ads_reward_click("skip");
        doneReward = done;
        if (MaxSdk.IsRewardedAdReady(IDReward))
        {
            DateTime tmp = Convert.ToDateTime(timeNewSessionReward);
            TimeSpan TimeSesion = DateTime.UtcNow - tmp;
            double Session = TimeSesion.TotalSeconds;
            timeNewSessionReward = DateTime.UtcNow.ToString();
            FirebaseManager.Instance?.Event_ads_reward_time_limit((float) Session);
            FirebaseManager.Instance?.Event_ads_reward_show("skip");
            FirebaseManager.Instance?.Event_ads_reward_offer("skip");
            FirebaseManager.Instance?.Ads_RewardShow(LevelControl.Instance.CurrentLevel);
            MaxSdk.ShowRewardedAd(IDReward);
        }
        else
        {
            //DialogManager.Instance.SpawnNoti();
        }
    }
    #endregion

    private string timeNewSessionInter;

    public float TimeLimitInter = 30;
    #region Inter
    System.Action doneInter;
    public void ShowInter(System.Action done, System.Action failed)
    {
        if (PlayerPrefs.GetInt("DeleteAds") == 1)
        {
            done?.Invoke();
            return;
        }

        if (GameData.Data.CurrentLevelMax <= 3)
        {
            done?.Invoke();
            return;
        }

        DateTime tmp = Convert.ToDateTime(timeNewSessionInter);
        TimeSpan TimeSesion = DateTime.UtcNow - tmp;
        double Session = TimeSesion.TotalSeconds;

        if (Session < TimeLimitInter)
        {
            done?.Invoke();
            return;
        }

        doneInter = done;
        if (MaxSdk.IsInterstitialReady(IDInter))
        {
            FirebaseManager.Instance?.Event_ad_inter_time_limit((float)Session);
            FirebaseManager.Instance?.Event_ad_inter_show();
            FirebaseManager.Instance?.Ads_InterShow(LevelControl.Instance.CurrentLevel);
            MaxSdk.ShowInterstitial(IDInter);
            Time.timeScale = 0;
        }
        else
        {
            failed?.Invoke();
            done?.Invoke();
            //Spawn Notification
        }
    }

    int retryAttemptInter;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
        // Load the first interstitial
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(IDInter);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        FirebaseManager.Instance?.Event_ad_inter_load();
        FirebaseManager.Instance?.Ads_InterRequest();
            retryAttemptInter = 0;
    }

    private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        FirebaseManager.Instance.Event_ad_impression(adUnitId, adInfo);
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        FirebaseManager.Instance?.Ads_InterFailed();
        retryAttemptInter++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptInter));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        timeNewSessionInter = DateTime.UtcNow.ToString();
        FirebaseManager.Instance?.Ads_InterComplete(LevelControl.Instance.CurrentLevel);
        doneInter?.Invoke();
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        FirebaseManager.Instance.Event_ad_inter_fail(errorInfo.Message);
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.Event_ad_inter_click();
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        Time.timeScale = 1;
        LoadInterstitial();
    }
    #endregion

    #region Banner
    public void BannerShow()
    {
        MaxSdk.ShowBanner(IDBanner);
    }

    public void BannerHide()
    {
        MaxSdk.HideBanner(IDBanner);
    }
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(IDBanner, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerWidth(IDBanner, 320);
        // Set background or background color for banners to be fully functional
        MaxSdk.SetBannerBackgroundColor(IDBanner, new Color32(100, 100, 100, 0));

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

        BannerShow();
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance?.Ads_BannerClick(LevelControl.Instance.CurrentLevel,"Game");
    }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance.Event_ad_impression(adUnitId, adInfo);
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
    #endregion

    #region OpenAd

    public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        FirebaseManager.Instance?.Ads_OpenComplete();
        MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        
    }

    private void OnAppOpenAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        FirebaseManager.Instance.Event_ad_impression(adUnitId, adInfo);
    }

    private string timeNewSessionOA;
    public void ShowAdIfReady()
    {
        if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
        {
            //Debug.LogError("Show");
            DateTime tmp = Convert.ToDateTime(timeNewSessionOA);
            TimeSpan TimeSesion = DateTime.UtcNow - tmp;
            double Session = TimeSesion.TotalSeconds;
            timeNewSessionOA = DateTime.UtcNow.ToString();
            FirebaseManager.Instance?.Event_ad_open_time_limit((float)Session);
            FirebaseManager.Instance?.Event_ad_open_show();
            FirebaseManager.Instance?.Ads_OpenShow("Home");
            MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
        }
        else
        {
            //Debug.LogError("Load");
            FirebaseManager.Instance?.Event_ad_open_load();
            MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
        }
    }

    private void OnAppOpenAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
    {
        FirebaseManager.Instance?.Event_ad_open_click();
    }

    private void OnAppOpenAdDisplayFailedEvent(string arg1, MaxSdkBase.ErrorInfo arg2, MaxSdkBase.AdInfo arg3)
    {
        FirebaseManager.Instance?.Event_ad_open_fail(arg2.Message);
        FirebaseManager.Instance?.Ads_OpenFailed("Home");
    }
    #endregion
}
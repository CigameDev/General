using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Analytics;
using System.Threading.Tasks;
using System;
using Firebase.Extensions;

public class FirebaseManager : Singleton<FirebaseManager>
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    public bool firebaseInitialized = false;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

    }

    #region CallStart
    // Handle initialization of the necessary firebase modules:
    private void InitializeFirebase()
    {
        //MessengeCallStart();
        Debug.Log("Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        Debug.Log("Set user properties.");
        // Set the user's sign up method.
        FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");

        // Set default session duration values.
        //FirebaseAnalytics.SetMinimumSessionDuration(new TimeSpan(0, 0, 10));
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

        firebaseInitialized = true;
        DisplayAnalyticsInstanceId();

        //Init_Invite();
        AnalyticsLogin();
        //MessengeCallStart();

        //FirebaseApp app = FirebaseApp.DefaultInstance;
        // Attach callbacks based on the ad format(s) you are using
        Event_open_app();
        ShowData();
        Debug.Log("Done Init Firebase");
    }

    public Task<string> DisplayAnalyticsInstanceId()
    {
        return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.Log("App instance ID fetch was canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log(String.Format("Encounted an error fetching app instance ID {0}",
                                        task.Exception.ToString()));
            }
            else if (task.IsCompleted)
            {
                Debug.Log(String.Format("App instance ID: {0}", task.Result));
            }
            return task;
        }).Unwrap();
    }


    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        Debug.Log("Logging a login event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    #endregion

    #region Event Game Analytic

    public void Event_level_start(int level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("level_start", new Parameter("level", level.ToString()));
        }
    }

    public void Event_level_fail(int level, int failcount)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("level_fail", new Parameter("level", level.ToString()), new Parameter("failcount", failcount.ToString()));
        }
    }

    public void Event_level_complete(int level, float timeplayed)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("level_complete", new Parameter("level", level.ToString()), new Parameter("timeplayed", timeplayed.ToString()));
        }
    }

    public void Event_open_app()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("open_app");
        }
    }

    public void Event_earn_virtual_currency(string virtual_currency_name, int value, string source)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("earn_virtual_currency",
                new Parameter("virtual_currency_name", virtual_currency_name), new Parameter("value", value.ToString()),
                new Parameter("source", source));
        }
    }

    public void Event_spend_virtual_currency(string virtual_currency_name, int value, string item_name)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("spend_virtual_currency",
            new Parameter("virtual_currency_name", virtual_currency_name), new Parameter("value", value.ToString()),
            new Parameter("source", item_name));
        }
    }

    public void Event_ads_reward_offer(string placement)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_offer", new Parameter("placement", placement));
        }
    }

    public void Event_ads_reward_click(string placement)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_click", new Parameter("placement", placement));
        }
    }

    public void Event_ads_reward_show(string placement)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_show", new Parameter("placement", placement));
        }
    }

    public void Event_ads_reward_fail(string placement, string errormsg)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_fail", new Parameter("placement", placement),
                new Parameter("errormsg", errormsg));
        }
    }

    public void Event_ads_reward_complete(string placement)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_complete", new Parameter("placement", placement));
        }
    }

    public void Event_ads_reward_time_limit(float second)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ads_reward_time_limit", new Parameter("second", second.ToString()));
        }
    }

    public void Event_ad_inter_fail(string errormsg)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_fail", new Parameter("errormsg", errormsg));
        }
    }

    public void Event_ad_inter_load()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_load");
        }
    }

    public void Event_ad_inter_show()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_show");
        }
    }

    public void Event_ad_inter_click()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_click");
        }
    }

    public void Event_ad_inter_time_limit(float second)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_inter_time_limit", new Parameter("second", second.ToString()));
        }
    }

    public void Event_ad_open_fail(string errormsg)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_open_fail", new Parameter("errormsg", errormsg));
        }
    }

    public void Event_ad_open_load()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_open_load");
        }
    }

    public void Event_ad_open_show()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_open_show");
        }
    }

    public void Event_ad_open_click()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_open_click");
        }
    }

    public void Event_ad_open_time_limit(float second)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("ad_open_time_limit", new Parameter("second", second.ToString()));
        }
    }

    public void Event_shop_open()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("shop_open");
        }
    }

    public void Event_ad_impression(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        if (firebaseInitialized)
        {
            double revenue = impressionData.Revenue;
            var impressionParameters = new[] {
                new Parameter("ad_platform", "AppLovin"),
                new Parameter("ad_source", impressionData.NetworkName),
                new Parameter("ad_unit_name", impressionData.AdUnitIdentifier),
                new Parameter("ad_format", impressionData.AdFormat),
                new Parameter("value", revenue.ToString()),
                new Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
            };
            FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        }
    }

    public void Event_IAP_offer(string IAP_package, string Offer_position)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("IAP_offer", new Parameter("IAP_package", IAP_package),
                new Parameter("Offer_position", Offer_position));
        }
    }

    public void Event_IAP_get(string IAP_package, string Offer_position)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("IAP_get", new Parameter("IAP_package", IAP_package),
                new Parameter("Offer_position", Offer_position));
        }
    }

    public void Event_IAP_cancel(string IAP_package, string Offer_position)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("IAP_cancel", new Parameter("IAP_package", IAP_package),
                new Parameter("Offer_position", Offer_position));
        }
    }

    public void Event_IAP_purchase_fail(string IAP_package, string Offer_position, string errormsg)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("IAP_purchase_fail", new Parameter("IAP_package", IAP_package),
                new Parameter("Offer_position", Offer_position), new Parameter("errormsg", errormsg));
        }
    }

    public void Event_IAP_purchase_success(string IAP_package, string Offer_position)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("IAP_purchase_success", new Parameter("IAP_package", IAP_package),
                new Parameter("Offer_position", Offer_position));
        }
    }

    public void Event_checkpoint_xx(int level)
    {
        if (firebaseInitialized)
        {
            if (PlayerPrefs.GetInt("CheckPoint" + level) == 0)
            {
                PlayerPrefs.SetInt("CheckPoint" + level, 1);
                FirebaseAnalytics.LogEvent("checkpoint_" + level.ToString("00"));
            }
        }
    }

    public void Level_Start(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Level_Start", new Parameter("Level", Level.ToString()));
        }
    }

    public void Level_Failed(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Level_Failed", new Parameter("Level", Level.ToString()));
        }
    }

    public void Level_Complete(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Level_Complete", new Parameter("Level", Level.ToString()));
        }
    }

    public void Ads_BannerClick(int Level, string Scene)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_BannerClick", new Parameter("Level", Level.ToString()), new Parameter("Scene", Scene));
        }
    }

    public void Ads_InterRequest()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_InterRequest");
        }
    }

    public void Ads_InterFailed()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_InterFailed");
        }
    }

    public void Ads_InterShow(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_InterShow", new Parameter("Level", Level.ToString()));
        }
    }

    public void Ads_InterComplete(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_InterComplete", new Parameter("Level", Level.ToString()));
        }
    }

    public void Ads_RewardRequest()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_RewardRequest");
        }
    }

    public void Ads_RewardFailed()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_RewardFailed");
        }
    }

    public void Ads_RewardShow(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_RewardShow", new Parameter("Level", Level.ToString()));
        }
    }

    public void Ads_RewardComplete(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_RewardComplete", new Parameter("Level", Level.ToString()));
        }
    }

    public void Ads_OpenRequest()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_OpenRequest");
        }
    }

    public void Ads_OpenShow(string Scene)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_OpenShow", new Parameter("Scene", Scene));
        }
    }

    public void Ads_OpenFailed(string Scene)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_OpenFailed", new Parameter("Scene", Scene));
        }
    }

    public void Ads_OpenComplete()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Ads_OpenComplete");
        }
    }

    public void Click_Setting(int Level)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Click_Setting", new Parameter("Level", Level.ToString()));
        }
    }

    public void Click_Replay()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Click_Replay");
        }
    }

    public void Click_RemoveAds()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Click_RemoveAds");
        }
    }

    public void Click_RemoveAdsSuccess()
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent("Op_Click_RemoveAdsSuccess");
        }
    }
    #endregion

    #region Messaging
    /*
    public void MessengeCallStart()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }
    */
    #endregion

    #region Remote Config
    // Start a fetch request.
    // FetchAsync only fetches new data if the current data is older than the provided
    // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
    // By default the timespan is 12 hours, and for production apps, this is a good
    // number. For this example though, it's set to a timespan of zero, so that
    // changes in the console will always show up immediately.

    public void FetchFireBase()
    {
        FetchDataAsync();
    }

    public void ShowData()
    {
        FetchFireBase();
    }

    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask =
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
            TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }
    //[END fetch_async]

    void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            Debug.Log("Fetch canceled.");
        }
        else if (fetchTask.IsFaulted)
        {
            Debug.Log("Fetch encountered an error.");
        }
        else if (fetchTask.IsCompleted)
        {
            Debug.Log("Fetch completed successfully!");
        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                .ContinueWithOnMainThread(task => {
                    Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                   info.FetchTime));
                    try
                    {
                        Debug.Log("Value inter_ad_capping_time: " + Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                            .GetValue("inter_ad_capping_time").StringValue);
                        Debug.Log("Value offline_play_on_off: " + Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance
                            .GetValue("offline_play_on_off").BooleanValue);
                        AdsController.Instance.TimeLimitInter = float.Parse(Firebase.RemoteConfig.FirebaseRemoteConfig
                            .DefaultInstance
                            .GetValue("inter_ad_capping_time").StringValue);
                        PlayerPrefs.SetInt("RemoteConfigNoInternet", Firebase.RemoteConfig.FirebaseRemoteConfig
                            .DefaultInstance
                            .GetValue("offline_play_on_off").BooleanValue
                            ? 0
                            : 1);
                    }
                    catch (Exception ex)
                    {
                        AdsController.Instance.TimeLimitInter = 30;
                        PlayerPrefs.SetInt("RemoteConfigNoInternet", 0);
                    }
                });

                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }

    #endregion
}

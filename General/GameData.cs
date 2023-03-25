using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    // user data
    public Action OnChangeCoin;
    public int OpenGameCount;
    public bool IsRated;

    /// <summary>
    /// The current level max. Start at 1
    /// </summary>
    public int CurrentLevelMax;

    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("DrCoin");
        }
        set
        {
            PlayerPrefs.SetInt("DrCoin", value);
            OnChangeCoin?.Invoke();
        }
    }
    //public LanguageType CurrentLanguageType;

    // setting data
    public bool IsSoundOn = true;
    public bool IsMusicOn = true;
    public bool IsVibrateOn = true;
    public bool IsBoughtNoAds = false;

    public string LastTimeCollectDailyReward = "";
    public int DayReward = 0;

    public GameData()
    {
        OpenGameCount = 0;
        IsRated = false;
        CurrentLevelMax = 1;
        Coin = 0;

        IsSoundOn = true;
        IsMusicOn = true;
        IsVibrateOn = true;

        IsBoughtNoAds = false;

        switch (Application.systemLanguage)
        {
            case SystemLanguage.Vietnamese:
                //CurrentLanguageType = LanguageType.Vietnamese;
                break;

            case SystemLanguage.English:
                //CurrentLanguageType = LanguageType.English;
                break;

            case SystemLanguage.Korean:
                //CurrentLanguageType = LanguageType.Korean;
                break;

            case SystemLanguage.Japanese:
                //CurrentLanguageType = LanguageType.Japanese;
                break;

            default:
                //CurrentLanguageType = LanguageType.English;
                break;
        }

        LastTimeCollectDailyReward = "";
        DayReward = 0;
    }

    private static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
        Array.ForEach(hashBytes, (hash) => hashString += Convert.ToString(hash, 16).PadLeft(2, '0'));

        return hashString.PadLeft(32, '0');
    }


    #region Static Method
    public static GameData Data { get; private set; }

    public static void LoadData()
    {
        string str = PlayerPrefs.GetString("FILE_SAVE_NAME");

        bool isReplaceData = false;
        if (!string.IsNullOrEmpty(str))
        {
            try
            {
                Data = JsonUtility.FromJson<GameData>(str);
            }
            catch (Exception)
            {
                isReplaceData = true;
                if (Data == null)
                {
                    try
                    {
                        byte[] decodedStringToByte = Convert.FromBase64String(str);
                        string decodedString = System.Text.Encoding.UTF8.GetString(decodedStringToByte);
                        Data = JsonUtility.FromJson<GameData>(decodedString);
                        string hash = PlayerPrefs.GetString("FILE_SAVE_HASH");
                        string hash_check = Md5Sum(decodedString + Data.CurrentLevelMax.ToString());
                        // check hash to detect hack
                        if (!hash.Equals(hash_check))
                        {
                            Debug.Log("Phat hien nghi van hack. Tao data moi");
                            Data = new GameData();
                        }
                    }
                    catch (System.Exception)
                    {
                        Debug.Log("Chua co data. Tao moi data");
                        Data = new GameData
                        {

                        };

                        //int allLevelInforLength = GameDatas.Instance.AllLevelInfor.Length;
                        //for (int i = 0; i < allLevelInforLength; i++)
                        //{
                        //    Data.LevelInformations.Add(new LevelInformation());
                        //}
                    }
                }
            }

            if (!isReplaceData)
            {
                string hash = PlayerPrefs.GetString("FILE_SAVE_HASH");
                string hash_check = Md5Sum(str + Data.CurrentLevelMax.ToString());
                // check hash to detect hack
                if (!hash.Equals(hash_check))
                {
                    Debug.Log("Phat hien nghi van hack. Tao data moi");
                    Data = new GameData();
                }
            }
        }
        else
        {
            Data = new GameData();
        }

        SaveData();
    }

    public static void SaveData()
    {
        string str = JsonUtility.ToJson(Data);
        byte[] toByteEncode = System.Text.Encoding.UTF8.GetBytes(str);
        string encodedText = Convert.ToBase64String(toByteEncode);

        PlayerPrefs.SetString("FILE_SAVE_HASH", Md5Sum(str + Data.CurrentLevelMax));
        PlayerPrefs.SetString("FILE_SAVE_NAME", encodedText);
        PlayerPrefs.Save();
    }

    public static void PassLevel(int level)
    {
        if (level == Data.CurrentLevelMax)
        {
            Data.CurrentLevelMax++;
            //EventDispatcherExtension.PostEvent(EventID.Pass_New_Level);
            SaveData();
        }
    }

    public static void ChangeLanguage(int indexNewLanguage)
    {
        SaveData();

        //Localize.Instance.ChangeLanguage();
        //EventDispatcherExtension.PostEvent(EventID.Language_Changed);
    }
    #endregion
}
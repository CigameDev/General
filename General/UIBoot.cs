using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Networking;

public class UIBoot : MonoBehaviour
{
    [SerializeField] private float timeLoading;
    [SerializeField] private Image imgLoadingBar;
    private AsyncOperation async;
    void Start()
    {
        StartCoroutine(PushInformation());
        GameData.LoadData();
        //SoundManager.Instance.SetVolumeMusic(GameData.Data.IsMusicOn);
        async = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        async.allowSceneActivation = false;
        Animation();
    }

    IEnumerator PushInformation()
    {
        const string url = "http://skymaregames.com:6666/Analytics";
        WWWForm form = new WWWForm();
        form.AddField("package", Application.identifier);
        form.AddField("DeviceID", SystemInfo.deviceUniqueIdentifier);

        UnityWebRequest w = UnityWebRequest.Post(url, form);
        yield return w.SendWebRequest();

        if (string.IsNullOrEmpty(w.error))
        {
            Debug.Log("Push Done");
        }
    }

    private void LoadBeginScene()
    {
        async.allowSceneActivation = true;
    }

    private Tween twe;
    private void Animation()
    {
        imgLoadingBar.DOFillAmount(1, timeLoading).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.5f, () => { LoadBeginScene(); });
        });
    }
}

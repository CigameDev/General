using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DialogManager : Singleton<DialogManager>
{
    [SerializeField] private Stack<Dialog> dialogStack = new Stack<Dialog>();
    public Transform dialogParent;

    [SerializeField] private Popup_Setting popupSetting;
    [SerializeField] private Popup_Answer popupAnswer;
    [SerializeField] private Popup_NoInternet popupNoInternet;
    [SerializeField] private Popup_QuitGame popupQuitGame;

    public float scaleToScreenX;
    public float scaleToScreenY;
    public bool isOnPopup()
    {
        return dialogStack.Count > 0;
    }

    public override void InitAwake()
    {
        scaleToScreenX = Screen.width / 1920f;
        scaleToScreenY = Screen.height / 1080f;

        var type = typeof(DialogManager);
        var fields = type.GetFields(BindingFlags.Instance|BindingFlags.NonPublic| BindingFlags.DeclaredOnly);

        foreach (var field in fields)
        {
            Dialog dialog = field.GetValue(this) as Dialog;

            if (dialog != null)
            {
                var dialogInstance = Instantiate(dialog, dialogParent);
                dialogInstance.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (dialogStack.Count > 0)
            {
                var topDialog = dialogStack.Peek();
                topDialog.OnBackPressed();
                //CloseDialog(topDialog);
            }
            else
            {
                //Preview Scenes Or Quit
                //if (LoadScenes.nameScene() == Contains.SceneWordMap)
                //    LoadScenes.LoadingScene(Contains.SceneMainMenu); 
                Popup_QuitGame.Instance.Show();
                Popup_QuitGame.Instance.CallStart();
            }
        }

    }


    public void OpenDialog(Dialog _dialog)
    {
        
        if (_dialog == null)
        {
            Debug.LogWarning("DIALOGMANAGER Open Dialog Error: invalid dialog");
            return;
        }

        if (_dialog.isShowAlone)
        {
            if (dialogStack.Count > 0)
            {
                foreach (var k in dialogStack)
                    k.Close();
            }
        }

        _dialog.Open();
        dialogStack.Push(_dialog);
        //_dialog.gameObject.transform.SetSiblingIndex(dialogStack.Count);
    }
    public void CloseDialog(Dialog _dialog)
    {
        if (_dialog == null)
            return;

        if (dialogStack.Count == 0)
        {
            Debug.LogWarning("DIALOGMANAGER Close Dialog ERROR: No dialog in stack!");
            return;
        }

        Dialog peekDialog = dialogStack.Peek();
        if (_dialog != peekDialog)
            return;

        Dialog topDialog = dialogStack.Pop();
        
        topDialog.Close();
        if (topDialog.isShowAlone)
        {
            if (dialogStack.Count > 0)
            {
                Dialog nextDialog = dialogStack.Peek();
                nextDialog.Open();
                Debug.Log("DialogManager Open next Dialog : " + nextDialog.gameObject.name);
            }
        }
    }

    public void ClearDialogs()
    {
        dialogStack.Clear();
    }

    public Dialog GetTopDialog()
    {
        return dialogStack.Peek();
    }

    public bool isShow()
    {
        return dialogStack.Count > 0;
    }
}


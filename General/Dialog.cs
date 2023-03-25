using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Dialog<T>: Dialog where T : Dialog<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    public override void Awake()
    {
        base.Awake();
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
        }
    }

    protected virtual void OnDestroy()
    {
        _instance = null;
    }

    public virtual void Show()
    {
        if (Instance != null)
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.OpenDialog(Instance);
            }
        }
    }
}

public class Dialog : MonoBehaviour
{
    public bool isShowAlone = true;
    public Action OnOpenComplete = delegate { };
    public Action OnCloseComplete = delegate { };
    public Action<Dialog> OnCloseStart = delegate { };

    [SerializeField] protected RectTransform popup;
    public virtual void Awake()
    {

    }

    Tween twOpen;
    protected bool KillDW = false;
    public void SetKill(bool _on) { KillDW = _on; }
    public virtual void Open()
    {
        if (twOpen != null||twClose!=null) return;
        if (popup == null)
        {
            gameObject.SetActive(true);
            OnOpenComplete?.Invoke();
            return;
        }
        if (KillDW)
        {
            gameObject.SetActive(true);
            OnOpenComplete?.Invoke();
            return;
        }
        gameObject.SetActive(true);
        popup.transform.localScale = Vector3.zero;
        twOpen= popup.DOScale(Vector3.one * 1.2f, 0.1f).OnComplete(() =>
        {
            popup.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                OnOpenComplete?.Invoke();
                twOpen = null;
            });
        });
    }

    Tween twClose;
    public virtual void Close()
    {
        if (twOpen != null || twClose != null ) return;
        if (popup == null)
        {
            gameObject.SetActive(false);
            OnCloseComplete?.Invoke();
            return;
        }
        if (KillDW)
        {
            gameObject.SetActive(false);
            OnCloseComplete?.Invoke();
            return;
        }
        twClose =popup.DOScale(Vector3.one * 1.2f, 0.1f).OnComplete(() =>
        {
            popup.DOScale(Vector3.zero, 0.1f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                OnCloseComplete?.Invoke();
                twClose = null;
            });
        });
    }

    public virtual void OnlyOneEventClose(Action action)
    {
        OnCloseComplete = null;
        OnCloseComplete += action;
    }

    public virtual void OnlyOneEventOpen(Action action)
    {
        OnOpenComplete = null;
        OnOpenComplete += action;
    }

    public virtual void OnBackPressed()
    {
        if (twOpen != null || twClose != null) return;
        SoundManager.Instance.PlayOnCamera(1);
        DialogManager.Instance.CloseDialog(this);
    }
}

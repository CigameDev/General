using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class UIButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
{
    public TypeAnimationButton Type;

    public RectTransform rectTransform
    {
        get { return transform.GetComponent<Image>().rectTransform; }
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    public UnityEvent ClickEvent = new UnityEvent();

    public void OnPointerUp(PointerEventData data)
    {
        AnimationUp();
    }

    public void OnPointerDown(PointerEventData data)
    {
        AnimationDown();
    }


    public void OnPointerClick(PointerEventData data)
    {
        AnimationClick();
    }

    private void AnimationIdle()
    {
        transform.DOKill();
        switch (Type)
        {
            case TypeAnimationButton.None:
                break;
            case TypeAnimationButton.Click:
                break;
            case TypeAnimationButton.InOut:
                transform.DOScale(Vector3.one * 0.9f, 0.2f).OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                    {
                        AnimationIdle();
                    });
                });
                break;
            case TypeAnimationButton.OutIn:
                transform.DOScale(Vector3.one * 1.1f, 0.2f).OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                    {
                        AnimationIdle();
                    });
                });
                break;
            case TypeAnimationButton.Bubble:
                transform.DOScale(Vector3.one * 1.1f, 0.4f).OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, 0.4f).OnComplete(() =>
                    {
                        AnimationIdle();
                    });
                });
                break;
        }
    }

    private void AnimationUp()
    {
        switch (Type)
        {
            case TypeAnimationButton.None:
                break;
            case TypeAnimationButton.Click:
                transform.localScale = Vector3.one;
                break;
            case TypeAnimationButton.InOut:
                break;
            case TypeAnimationButton.OutIn:
                break;
            case TypeAnimationButton.Bubble:
                break;
        }
    }

    private void AnimationDown()
    {
        switch (Type)
        {
            case TypeAnimationButton.None:
                break;
            case TypeAnimationButton.Click:
                transform.localScale = Vector3.one * 1.1f;
                break;
            case TypeAnimationButton.InOut:
                break;
            case TypeAnimationButton.OutIn:
                break;
            case TypeAnimationButton.Bubble:
                break;
        }
    }

    private void AnimationClick()
    {
        transform.DOKill();
        switch (Type)
        {
            case TypeAnimationButton.None:
                SoundManager.Instance.PlayFx();
                ClickEvent.Invoke();
                break;
            case TypeAnimationButton.Click:
                SoundManager.Instance.PlayFx();
                ClickEvent.Invoke();
                break;
            case TypeAnimationButton.InOut:
                transform.DOScale(Vector3.one * 0.8f, 0.2f).OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                    {
                        SoundManager.Instance.PlayFx();
                        ClickEvent.Invoke();
                        AnimationIdle();
                    });
                });
                break;
            case TypeAnimationButton.OutIn:
                transform.DOScale(Vector3.one * 0.9f, 0.2f).OnComplete(() =>
                {
                    transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
                    {
                        SoundManager.Instance.PlayFx();
                        ClickEvent.Invoke();
                        AnimationIdle();
                    });
                });
                break;
            case TypeAnimationButton.Bubble:
                transform.DOScale(RetureVector3(1.3f, 0.7f, 1), 0.14f).OnComplete(() =>
                {
                    transform.DOScale(RetureVector3(0.75f, 1.25f, 1), 0.13f).OnComplete(() =>
                    {
                        transform.DOScale(RetureVector3(1.1f, 0.85f, 1), 0.1f).OnComplete(() =>
                        {
                            transform.DOScale(RetureVector3(0.85f, 1.1f, 1), 0.08f).OnComplete(() =>
                            {
                                transform.DOScale(RetureVector3(1.05f, 0.9f, 1), 0.06f).OnComplete(() =>
                                {
                                    transform.DOScale(RetureVector3(0.9f, 1.05f, 1), 0.05f).OnComplete(() =>
                                    {
                                        transform.DOScale(RetureVector3(1, 1, 1), 0.04f).OnComplete(() =>
                                        {
                                            SoundManager.Instance.PlayFx();
                                            ClickEvent.Invoke();
                                            AnimationIdle();
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
                break;
        }
    }

    private Vector3 RetureVector3(float x, float y, float z)
    {
        return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
    }
    public void SetUpEvent(UnityAction action)
    {
        AnimationIdle();
        ClickEvent.RemoveAllListeners();
        ClickEvent.AddListener(action);
    }
}

public enum TypeAnimationButton
{
    None,
    Click,
    Bubble,
    InOut,
    OutIn
}
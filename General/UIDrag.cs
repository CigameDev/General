using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIDrag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler,IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private bool checkDrop;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetCanvas(Canvas _canvas)
    {
        canvas = _canvas;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        checkDrop = false;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        onDragBegin?.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        onDragEnd?.Invoke();
        if (!checkDrop)
        {
            onDragEndFailed?.Invoke();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(canvas == null) Debug.LogError("Canvas Null: Please Call Method SetCanvas");
        else
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    private Action<GameObject> onDrop;
    private Action onDragBegin;
    private Action onDragEnd;
    private Action onDragEndFailed;

    public void SetUpEventDrop(Action<GameObject> _onDrop)
    {
        onDrop = _onDrop;
    }

    public void SetUpEventDragBegin(Action _onDragBegin)
    {
        onDragBegin = _onDragBegin;
    }

    public void SetUpEventDragEnd(Action _onDragEnd)
    {
        onDragEnd = _onDragEnd;
    }

    public void SetUpEventDragEndFailed(Action _onDragEndFailed)
    {
        onDragEndFailed = _onDragEndFailed;
    }

    public void CallOnDrop(GameObject drop)
    {
        checkDrop = true;
        onDrop?.Invoke(drop);
    }
}

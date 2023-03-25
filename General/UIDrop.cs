using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventDrop?.Invoke();
            if (eventData.pointerDrag.GetComponent<UIDrag>() != null)
            {
                eventData.pointerDrag.GetComponent<UIDrag>().CallOnDrop(gameObject);
            }
        }
    }

    private Action eventDrop;
    public void SetUpEventDrop(Action _eventDrop)
    {
        eventDrop = _eventDrop;
    }
}

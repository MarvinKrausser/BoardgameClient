using System.Collections;
using System.Collections.Generic;
using communication;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/// <summary>
/// Handles the drag function of the cards
/// </summary>
public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform parentToReturnTo = null;
    public Card cardType;
    public GameObject cardBack;
    
    /// <summary>
    /// Called when a card starts being dragged
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {

        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>
    /// Called while the card is dragged
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {

        this.transform.position = eventData.position;
    }

    /// <summary>
    /// Called at the end of a card drag
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {

        this.transform.SetParent(parentToReturnTo);
       GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

   
    
}

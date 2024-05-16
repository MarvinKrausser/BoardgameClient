using System.Collections;
using System.Collections.Generic;
using System.Linq;
using communication;
using managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class is representative for the slots where a card can be dropped in
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventdata)
    {
       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        
    }
    
    /// <summary>
    /// Called when a card is dropped on a slot 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        

        
         Dragable d = eventData.pointerDrag.GetComponent<Dragable>();
         
         if (d != null)
         {

             if (transform.childCount == 1)
             {
                 Debug.Log("Max card limit reached");
                 return;
             }
             
             d.parentToReturnTo = this.transform;
             d.transform.SetParent(this.transform);
             
             //Card[] selectedCards = GetSelectedCards();
             //writeMessage.WriteMessageCARD_CHOICE(selectedCards);
         }

    }
    
}

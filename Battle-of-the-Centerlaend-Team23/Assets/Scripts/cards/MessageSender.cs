using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;
using managers;
using UnityEngine.UI;

/// <summary>
/// Creates a message from the selected cards and sends them to the server 
/// </summary>
public class MessageSender : MonoBehaviour
{
    public List<Card> selectedCards = new List<Card>();
    
    [SerializeField] public Button confirmButton;

    public static Button buttonInstance;
    

    /// <summary>
    /// Fetches the selected cards from the GameScene and adds them to the selectedCards List.
    /// </summary>
    /// <returns></returns>
    public Card[] GetSelectedCards()
    {
        selectedCards.Clear();
        foreach (Transform cardTransform in transform)
        {
            Dragable card = cardTransform.GetComponentInChildren<Dragable>();
            if (card != null)
            {
                selectedCards.Add(card.cardType);
                
            }
            else
            {
                selectedCards.Add(Card.EMPTY);  // Füge den Wert "Empty" hinzu, wenn keine Karte vorhanden ist
            }
        }

        return selectedCards.ToArray();
    }

    /// <summary>
    /// Method is triggered on the button click in the GameScene.
    /// Creates a string from the selected cards and sends them to the server with the MessageManager instance
    /// </summary>
    public void OnSendButtonClicked()
    {
        Card[] cards = GetSelectedCards();
        Debug.Log("CARD_CHOICE_MESSAGE "+ " " + CardListToString() + " ");
        MessageManager.instance._writeMessage.WriteMessageCARD_CHOICE(cards);

        /*foreach (Transform cardTransform in transform)
        {
            Destroy(cardTransform.gameObject);
        }*/
        
        selectedCards.Clear();
        
        ColorBlock cb = buttonInstance.colors;
        cb.normalColor = new Color(255, 255, 255, 10);
        buttonInstance.colors = cb;
        confirmButton.interactable = false;
    }

    /// <summary>
    /// Reformat the selected cards to a string
    /// </summary>
    /// <returns></returns>
    public string CardListToString()
    {
        string output = " ";
        foreach (var card in selectedCards)
        {
            output += card.ToString();
        }

        return output;
    }

    private void Awake()
    {
        buttonInstance = confirmButton;
    }
}


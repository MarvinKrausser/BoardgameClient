using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using communication;
using managers;

/// <summary>
/// Class to display the cards in the GameScene
/// </summary>
public class DisplayingCards : MonoBehaviour
{
    public static DisplayingCards instance;
    
    public GameObject cardPrefab;
    public float cardWidth = 2f;

    public List<GameObject> cards = new List<GameObject>();
    
    /// <summary>
    /// Creates GameObjects for each card and sets the corresponding sprites for each of them
    /// </summary>
    /// <param name="cardList"></param>
    public void DisplayCards(List<Card> cardList)
    {
        
        for(int i = 0; i < cardList.Count; i++)
        {
            CardData data = CardDataBase.Get(cardList[i]);
            
            // Instantiate a card
            GameObject card = Instantiate(cardPrefab, transform);
            card.GetComponent<Dragable>().cardType = cardList[i];

            // Position it in a row
            card.transform.position = new Vector3(i * cardWidth, 0, 0);

            // Assign the card's data based on the enum
            ThisCard cardScript = card.GetComponent<ThisCard>();
            cardScript.UpdateData(data);
            card.SetActive(true);
            cards.Add(card);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        MessageManager.cardOfferSemephore.Release();
    }

}

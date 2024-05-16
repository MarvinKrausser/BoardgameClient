using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using communication;

namespace Tests.PlayMode
{
    public class SendCardsTest
    {
        [Test]
        public void GetSelectedCardsTest()
        {
            var messageSender = new GameObject().AddComponent<MessageSender>();
        
            var card1 = new GameObject().AddComponent<Dragable>();
            card1.cardType = Card.MOVE_1;
            card1.transform.SetParent(messageSender.transform);
        
            var card2 = new GameObject().AddComponent<Dragable>();
            card2.cardType = Card.MOVE_2;
            card2.transform.SetParent(messageSender.transform);
        
            var card3 = new GameObject().AddComponent<Dragable>();
            card3.cardType = Card.MOVE_3;
            card3.transform.SetParent(messageSender.transform);
        
            var emptyCard = new GameObject().AddComponent<Dragable>();
            emptyCard.cardType = Card.EMPTY;
            emptyCard.transform.SetParent(messageSender.transform);
        
            var selectedCards = messageSender.GetSelectedCards();
        
            Assert.AreEqual(4, selectedCards.Length);
            Assert.Contains(Card.MOVE_1, selectedCards);
            Assert.Contains(Card.MOVE_2, selectedCards);
            Assert.Contains(Card.MOVE_3, selectedCards);
            Assert.Contains(Card.EMPTY, selectedCards);
        }
    
        [Test]
        public void CardListToStringTest()
        {
            var messageSender = new GameObject().AddComponent<MessageSender>();
            messageSender.selectedCards = new List<Card>()
            {
                Card.MOVE_1,
                Card.MOVE_2,
                Card.MOVE_3,
                Card.EMPTY
            };
        
            var output = messageSender.CardListToString();
        
            Assert.AreEqual(" MOVE_1MOVE_2MOVE_3EMPTY", output);
        }
    }
}


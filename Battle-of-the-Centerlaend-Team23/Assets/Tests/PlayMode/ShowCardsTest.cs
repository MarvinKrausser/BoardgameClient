using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class ShowCardsTest
    {
        [Test]
        public void UpdateDataTest()
        {
            var thisCard = new GameObject().AddComponent<ThisCard>();
            var cardData = new CardData();
            var sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
            var moveSprite = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        
            // Set the properties of cardData directly
            cardData.CardName = "Card Name";
            cardData.CardDescription = "Card Description";
        
            thisCard.UpdateData(cardData);
        
            if (thisCard.data != null) Assert.AreEqual(cardData, thisCard.data.Value);
        }
    }
}
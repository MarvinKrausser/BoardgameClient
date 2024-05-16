using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using communication;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static communication.Card;

/// <summary>
/// Loads the sprites of a card
/// </summary>
public struct CardData
{
    public string CardName;
    public Card EnumName;
    public string CardDescription;
    public string spritePath;
    public string movepath;

    public Sprite GetSprite()
    {
        // Debug.Log("Loading Sprite at path '" + this.spritePath + "' ");
        return Resources.Load<Sprite>(this.spritePath);
    }

    public Sprite GetMoveSprite()
    {
        return Resources.Load<Sprite>(this.movepath);
    }
}

/// <summary>
/// Returns sprites, description and name of each card based of the incoming enum 
/// </summary>
public class CardDataBase : MonoBehaviour
{
    public static CardData Get(Card type)
    {
        switch (type)
        {
           case Card.MOVE_1:
                return new CardData
                {
                    CardName = "MOVE_1",
                    EnumName = Card.MOVE_1,
                    CardDescription = "MOVE 1 FORWARD",
                    spritePath = "MOVE_1",
                    movepath = "Eins"
                };

            case Card.MOVE_2:
                return new CardData
                {
                    CardName = "MOVE_2",
                    EnumName = Card.MOVE_2,
                    CardDescription = "MOVE 2 FORWARD",
                    spritePath = "MOVE_2",
                    movepath = "Zwei"
                    
                };

            case Card.MOVE_3:
                return new CardData
                {
                    CardName = "MOVE_3",
                    EnumName = Card.MOVE_3,
                    CardDescription = "MOVE 2 FORWARD",
                    spritePath = "MOVE_3",
                    movepath = "Drei"
                };

            case Card.MOVE_BACK:
                return new CardData
                {
                    CardName = "MOVE_BACK",
                    EnumName = Card.MOVE_BACK,
                    CardDescription = "MOVE TO BACK",
                    spritePath = "BACK",
                    movepath = "1"
                };

            case Card.U_TURN:
                return new CardData
                {
                    CardName = "U_TURN",
                    EnumName = Card.U_TURN,
                    CardDescription = "U_TURN",
                    spritePath = "U_TURN",
                    movepath = "1"
                };

            case Card.LEFT_TURN:
                return new CardData
                {
                    CardName = "LEFT_TURN",
                    EnumName = Card.LEFT_TURN,
                    CardDescription = "TURN TO LEFT",
                    spritePath = "TURN_LEFT",
                    movepath = "1"
                };

            case Card.RIGHT_TURN:
                return new CardData
                {
                    CardName = "RIGHT_TURN",
                    EnumName = Card.RIGHT_TURN,
                    CardDescription = "TURN TO RIGHT",
                    spritePath = "TURN_RIGHT",
                    movepath = "1"
                };

            case Card.AGAIN:
                return new CardData
                {
                    CardName = "AGAIN",
                    EnumName = Card.AGAIN,
                    CardDescription = "USE IT AGAIN",
                    spritePath = "AGAIN",
                    movepath = "1"
                };

            case Card.LEMBAS:
                return new CardData
                {
                    CardName = "LEMBAS",
                    EnumName = Card.LEMBAS,
                    CardDescription = "GET LEMBAS",
                    spritePath = "LEMBAS",
                    movepath = "1"
                };

            case Card.EMPTY:
                return new CardData
                {
                    CardName = "EMPTY",
                    EnumName = Card.EMPTY,
                    CardDescription = "EMPTY",
                    spritePath = "Empty",
                    movepath = "1"
                };

            default:
                throw new InvalidEnumArgumentException("BUG: Invalid Card Enum Value!");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using communication;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor;


/// <summary>
/// Representative for a single card in the GameScene. Handles the sprites and text of the card.
/// </summary>
public class ThisCard : MonoBehaviour
{
    public Text nameText, descriptionText;
    public CardData? data;
    public Image rotationImage;
    public Image MoveImage;
    
  
    /// <summary>
    /// Updates the sprites and data of a card
    /// </summary>
    /// <param name="data"></param>
    public void UpdateData(CardData data)
    {
        var sprite = data.GetSprite();
        var spritemove = data.GetMoveSprite();

        if (rotationImage != null && sprite != null)
        {
            this.rotationImage.sprite = sprite;
        }
        
        if (MoveImage != null && spritemove != null)
        {
            this.MoveImage.sprite = spritemove;
        }

        if (sprite == null)
        {
            Debug.Log("BUG: Sprite at path '" + data.spritePath + "' is null!");
        }
        
        this.data = data;
        
        
    }
    
    // Update is called once per frame
    /// <summary>
    /// Updates the texts of the card
    /// </summary>
    void Update()
    {
        
        if (this.data != null)
        {
            nameText.text = this.data.Value.CardName ;
            descriptionText.text = this.data.Value.CardDescription ;
        }
       
    }
}

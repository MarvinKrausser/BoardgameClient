using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using communication;
using managers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the selectable character from the CharacterChoice message and sends the chosen character to the server.
/// </summary>

public class CharacterSelectionUI : MonoBehaviour {
    
    [SerializeField] private Sprite Galadriel;
    [SerializeField] public Sprite Gandalf;
    [SerializeField] private Sprite Sam;
    [SerializeField] private Sprite Gollum;
    [SerializeField] private Sprite Baumbart;
    [SerializeField] private Sprite Arwen;
    [SerializeField] private Sprite Legolas;
    [SerializeField] private Sprite Boromir;
    [SerializeField] private Sprite Gimli;
    [SerializeField] public Sprite Pippin;
    [SerializeField] private Sprite Merry;
    [SerializeField] private Sprite Frodo;
    [SerializeField] private Sprite Aragorn;
    
    
    public static Character[] characterForSelect;
    public static CharacterSelectionUI instance;

    public static Character character1;
    public static Character character2;
    public static bool charactersChanged;
    

    public CharacterUI[] characters= {};

    public static CharacterUI currentCharacter;
    
    

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// sets the first character to the selected one
    /// </summary>
    public void Start() {
        //überprüfen ob min. 1 charakter gesendet wird
        if (characters.Length > 0) { 
            //erste character = aktuelle charakter
            currentCharacter = characters[0];
        }
        setCharacters();
    }


    //sendet den ausgewählten charakter
    /// <summary>
    /// sends the selected character to the server
    /// </summary>
    public void sendCharacter()
    {
        MessageManager.instance._writeMessage.WriteMessageCHARACTER_CHOICE((Character)Enum.Parse(typeof(Character), currentCharacter.name));
        Debug.Log("Auswahl wurde gesendet!");
    }
    
    //wenn ein Charakter ausgewählt wird ->  setzt den aktuellen Charakter auf den übergebenen Wert.
    /// <summary>
    /// sets the selectet character as the currentCaracter
    /// </summary>
    /// <param name="character"></param>
    public void SetCharacter(CharacterUI character) {
        currentCharacter = character;
    }

    //setzt den gesendeten charakter in die charakteranzeige ein
    /// <summary>
    /// what the ... idk man 
    /// </summary>
    /// <param name="char1"></param>
    /// <param name="char2"></param>
    public static void changeCharacters(Character char1, Character char2)
    {
        character1 = char1;
        character2 = char2;
        charactersChanged = true;
    }
    
    
    //aktualisiert die Anzeige der Charaktere
    //setzt die Icons und Namen der Charaktere basierend auf den ausgewählten Charakteren.
    /// <summary>
    /// Updates the UI for the selected character.
    /// </summary>
    public void setCharacters()
    {
        characters = new CharacterUI[2];

        // Erstelle die CharacterUI-Objekte und weise sie den entsprechenden Indizes zu
        characters[0] = new CharacterUI();
        characters[1] = new CharacterUI();


        characters[0].icon = CharacterEnumToTexture(character1);
        characters[0].name = character1.ToString();
        characters[1].icon = CharacterEnumToTexture(character2);
        characters[1].name = character2.ToString();
        
    }
    
    //Wandelt die enums in sprites um 
    /// <summary>
    /// Uses the enum to get the correct sprite of the character
    /// </summary>
    /// <param name="enumeration"></param>
    /// <returns></returns>
    public Sprite CharacterEnumToTexture(Character enumeration)
    {
        Sprite characterImage;

        switch (enumeration)
        {
            case Character.GANDALF:
                characterImage = Gandalf;
                break;
            case Character.GALADRIEL:
                characterImage = Galadriel;
                break;
            case Character.SAM:
                characterImage = Sam;
                break;
            case Character.ARWEN:
                characterImage = Arwen;
                break;
            case Character.FRODO:
                characterImage = Frodo;
                break;
            case Character.BOROMIR:
                characterImage = Boromir;
                break;
            case Character.GOLLUM:
                characterImage = Gollum;
                break;
            case Character.MERRY:
                characterImage = Merry;
                break;
            case Character.ARAGORN:
                characterImage = Aragorn;
                break;
            case Character.GIMLI:
                characterImage = Gimli;
                break;
            case Character.PIPPIN:
                characterImage = Pippin;
                break;
            case Character.BAUMBART:
                characterImage = Baumbart;
                break;
            case Character.LEGOLAS:
                characterImage = Legolas;
                break;
            default:
                characterImage = null;
                break;
          
        }
        return characterImage;

    }

}
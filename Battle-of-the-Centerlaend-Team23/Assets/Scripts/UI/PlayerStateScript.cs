using System;
using System.Threading;
using communication;
using managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Displays the stats for each player as an UI element in the GameScene.
    /// </summary>
    public class PlayerStateScript : MonoBehaviour
    {
        [Header("Player States")]
        [SerializeField] private GameObject[] _playerStatePrefabs;
        
        [Header("Prefab Text Fields")]
        [SerializeField] private TMP_Text[] _names;
        [SerializeField] private TMP_Text[] _lives;
        [SerializeField] private TMP_Text[] _lembas;
        [SerializeField] private TMP_Text[] _checkpoint;
        
        [Header("Prefab Sprite Lists")]
        [SerializeField] private Image[] _currentCharacter;
        [SerializeField] private Image[] _currentCard;

        [Header("Given Sprites Lists")]
        [SerializeField] private Sprite[] _givenCharacters;
        [SerializeField] private Sprite[] _givenCards;
        
        [Header("UI Permanents")]
        [SerializeField] private TMP_Text _roundCount;
        
        /// <summary>
        /// Takes a incoming Game_State message and updates the UI to the given values
        /// </summary>
        /// <param name="message"></param>
        public void StateUpdate(GAME_STATE_Message_Data message)
        {

            int countDeath = 0;
            int playerCount = message.playerStates.Count;
            Debug.Log("PlayerCount: " + playerCount);
            int i;
            
            for (i = 0; i < playerCount; i++)
            {
                //get player information
                int turnOrder = message.playerStates[i].turnOrder;
                string playerName = message.playerStates[i].playerName;
                int lives = message.playerStates[i].lives;
                int lembas = message.playerStates[i].lembasCount;
                int checkpoint = message.playerStates[i].reachedCheckpoints;
                string character = message.playerStates[i].character.ToString();

                //check if player's dead
                if (turnOrder == -1)
                {
                    turnOrder = playerCount - ++countDeath;
                }
                
                //Debug.Log("turnOrder is: " + turnOrder);
                
                //call InsertStates
                //InsertStates(playerName, lives, lembas, checkpoint, i);
                InsertStates(playerName, lives, lembas, checkpoint, character, turnOrder);
                //DeleteCards(turnOrder);
            }

            //deactivate unused prefabs
            while (i < _playerStatePrefabs.Length)
            {
                if (_playerStatePrefabs[i].activeSelf)
                {
                    _playerStatePrefabs[i].SetActive(false);
                }

                i++;
            }
            //update current round display
            _roundCount.text = message.currentRound.ToString();
        }

        /// <summary>
        /// Takes a incoming Card_Event and updates the UI to the given values.
        /// </summary>
        /// <param name="message"></param>
        public void StateUpdate(CARD_EVENT_Message_Data message)
        {
            int playerCount = message.playerStates[message.playerStates.Length - 1].Count;
            Debug.Log("PlayerCount: " + playerCount);
            int countDeath = 0;
            int i;
            
            for (i = 0; i < playerCount; i++)
            {

                //Debug.Log(" i = " + i + " und playerCount = " + playerCount);
                //get player information
                int turnOrder = message.playerStates[message.playerStates.Length - 1][i].turnOrder;
                string playerName = message.playerStates[message.playerStates.Length - 1][i].playerName;
                int lives = message.playerStates[message.playerStates.Length - 1][i].lives;
                int lembas = message.playerStates[message.playerStates.Length - 1][i].lembasCount;
                int checkpoint = message.playerStates[message.playerStates.Length - 1][i].reachedCheckpoints;
                string character = message.playerStates[message.playerStates.Length - 1][i].character.ToString();
                string card = message.card.ToString();

                string nameForCard = message.playerName;

                //check if player's dead
                if (turnOrder == -1)
                {
                    turnOrder = playerCount - ++countDeath;
                }

                //check for player who actually played the card to not fucking show the played card for every single player existing in this game
                if (playerName == nameForCard)
                {
                    InsertCardSprites(card, turnOrder);
                }

                //Debug.Log("turnOrder is: " + turnOrder);

                //call InsertStates
                //InsertStates(playerName, lives, lembas, checkpoint, j);
                InsertStates(playerName, lives, lembas, checkpoint, character, turnOrder);
            }

            while (i < _playerStatePrefabs.Length)
            {
                if (_playerStatePrefabs[i].activeSelf)
                {
                    _playerStatePrefabs[i].SetActive(false);
                }

                i++;
            }
        }

        /// <summary>
        /// Updates the UI on a incoming River_Event message
        /// </summary>
        /// <param name="message"></param>
        public void StateUpdate(RIVER_EVENT_Message_Data message)
        {
            int playerCount = message.playerStates[message.playerStates.Length - 1].Length;
            
            Debug.Log("PlayerCount: " + playerCount);
            int countDeath = 0;
            int i;
            
            for (i = 0; i < playerCount; i++)
            {
                //get player information
                int turnOrder = message.playerStates[message.playerStates.Length - 1][i].turnOrder;
                string playerName = message.playerStates[message.playerStates.Length - 1][i].playerName;
                int lives = message.playerStates[message.playerStates.Length - 1][i].lives;
                int lembas = message.playerStates[message.playerStates.Length - 1][i].lembasCount;
                int checkpoint = message.playerStates[message.playerStates.Length - 1][i].reachedCheckpoints;
                string character = message.playerStates[message.playerStates.Length - 1][i].character.ToString();

                //check if player's dead
                if (turnOrder == -1)
                {
                    turnOrder = playerCount - ++countDeath;
                }

                //call InsertStates
                //InsertStates(playerName, lives, lembas, checkpoint, j);
                InsertStates(playerName, lives, lembas, checkpoint, character, turnOrder);
            }

            //deactivate unused prefabs
            while (i < _playerStatePrefabs.Length)
            {
                if (_playerStatePrefabs[i].activeSelf)
                {
                    _playerStatePrefabs[i].SetActive(false);
                }

                i++;
            }
        }
        
        /// <summary>
        /// Updates the UI on a incoming Shot_Event message
        /// </summary>
        /// <param name="message"></param>
        public void StateUpdate(SHOT_EVENT_Message_Data message)
        {
            int playerCount = message.playerStates.Count;
            Debug.Log("PlayerCount: " + playerCount);
            int countDeath = 0;
            int i;

            for (i = 0; i < message.playerStates.Count; i++)
            {
                //get player information
                int turnOrder = message.playerStates[i].turnOrder;
                string playerName = message.playerStates[i].playerName;
                int lives = message.playerStates[i].lives;
                int lembas = message.playerStates[i].lembasCount;
                int checkpoint = message.playerStates[i].reachedCheckpoints;
                string character = message.playerStates[i].character.ToString();

                //check if player's dead
                if (turnOrder == -1)
                {
                    turnOrder = playerCount - ++countDeath;
                }

                //call InsertStates
                //InsertStates(playerName, lives, lembas, checkpoint, j);
                InsertStates(playerName, lives, lembas, checkpoint, character, turnOrder);
            }

            while (i < _playerStatePrefabs.Length)
            {
                if (_playerStatePrefabs[i].activeSelf)
                {
                    _playerStatePrefabs[i].SetActive(false);
                }

                i++;
            }
        }
        
        private void InsertStates(string newName, int newLives, int newLembas, int newCheckpoint, string character, int turnOrder)
        {
            //set new stats
            _names[turnOrder].text = newName;

            _lives[turnOrder].text = newLives.ToString();

            _lembas[turnOrder].text = newLembas.ToString();

            _checkpoint[turnOrder].text = newCheckpoint.ToString();
            
            //call InsertCharacterSprites
            InsertCharacterSprites(character, turnOrder);
        }

        private void InsertCharacterSprites(string character, int turnOrder)
        {
            Sprite characterSprite = null;
            switch (character)
            {
                case "ARAGORN": characterSprite = _givenCharacters[0];
                    break;
                case "ARWEN": characterSprite = _givenCharacters[1];
                    break;
                case "BAUMBART": characterSprite = _givenCharacters[2];
                    break;
                case "BOROMIR": characterSprite = _givenCharacters[3];
                    break;
                case "FRODO": characterSprite = _givenCharacters[4];
                    break;
                case "GALADRIEL": characterSprite = _givenCharacters[5];
                    break;
                case "GANDALF": characterSprite = _givenCharacters[6];
                    break;
                case "GIMLI": characterSprite = _givenCharacters[7];
                    break;
                case "GOLLUM": characterSprite = _givenCharacters[8];
                    break;
                case "LEGOLAS": characterSprite = _givenCharacters[9];
                    break;
                case "MERRY": characterSprite = _givenCharacters[10];
                    break;
                case "PIPPIN": characterSprite = _givenCharacters[11];
                    break;
                case "SAM": characterSprite = _givenCharacters[12];
                    break;
                default: Debug.Log("Check Character Method");
                    break;
            }
            _currentCharacter[turnOrder].overrideSprite = characterSprite;
        }
        private void InsertCardSprites(string card, int turnOrder)
        {
            Sprite cardSprite = null;
            switch (card)
            {
                case "MOVE_1": cardSprite = _givenCards[0];
                    break;
                case "MOVE_2": cardSprite = _givenCards[1];
                    break;
                case "MOVE_3": cardSprite = _givenCards[2];
                    break;
                case "LEFT_TURN": cardSprite = _givenCards[3];
                    break;
                case "RIGHT_TURN": cardSprite = _givenCards[4];
                    break;
                case "MOVE_BACK": cardSprite = _givenCards[5];
                    break;
                case "U_TURN": cardSprite = _givenCards[6];
                    break;
                case "AGAIN": cardSprite = _givenCards[7];
                    break;
                case "LEMBAS": cardSprite = _givenCards[8];
                    break;
                default: Debug.Log("Check Card Method");
                    break;
            }
            _currentCard[turnOrder].sprite = cardSprite;
        }

        private void Start()
        {
            MessageManager._playerStateScript = this;
        }
    }
}
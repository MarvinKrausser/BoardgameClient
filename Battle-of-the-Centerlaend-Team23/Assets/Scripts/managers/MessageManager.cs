using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using communication;
using Newtonsoft.Json;
using UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace managers
{
    /// <summary>
    /// Holds all the events that get triggered on a incoming message. All behaviour and methods that should happen on an incoming message are called in here.
    /// This class also syncs the flow of the game.
    /// </summary>
    public class MessageManager : MonoBehaviour
    {
        public static MessageManager instance;
        
        private Communication _communication;
        private ReadMessage _readMessage;
        public WriteMessage _writeMessage;

        private static SceneSwitcher _sceneSwitcher;
        public static PlayerStateScript _playerStateScript;

        private bool _changeSceneToWaitingOnOtherPlayers = false;
        public bool _changeSceneToCharacterSelection = false;
        public bool _changeSceneToGame = false;
        private bool _onRoundStart = false;
        private bool _onGameEnd = false;
        
        private bool errorCode6;
        private bool errorCode7;
        
        private bool _firstHello_Client = false;
        private bool _firstParticipantsInfo = false;

        public bool tryingToReconnect = false;

        public CARD_OFFER_Message _cardOfferMessage = null;
        private PARTICIPANTS_INFO_Message _participantsInfoMessage = null;
        private ROUND_START_Message _roundStartMessage = null;
        private GAME_STATE_Message _gameStateMessage = null;
        private SHOT_EVENT_Message _shotEventMessage = null;
        private RIVER_EVENT_Message _riverEventMessage = null;
        private EAGLE_EVENT_Message _eagleEventMessage = null;
        private CARD_EVENT_Message _cardEventMessage = null;
        private GAME_END_Message _gameEndMessage = null;

        private INVALID_MESSAGE_Message _invalidMessage = null;
        //private PAUSED_Message _pausedMessage = null;

        public static string activeScene;

        public static SemaphoreSlim gameStateSemaphore = new SemaphoreSlim(0);
        public static SemaphoreSlim participantsInfoSemaphore = new SemaphoreSlim(0);
        public static SemaphoreSlim reconnectSemephore = new SemaphoreSlim(0);
        public static SemaphoreSlim cardOfferSemephore = new SemaphoreSlim(0);
        public static SemaphoreSlim reconnectFromConnectionSceneSemaphore = new SemaphoreSlim(0);

        public static int connectionAttempts;
        public static bool switchToConnectionLost;

        /// <summary>
        /// this method should be called when you want to open the websocket
        /// </summary>
        public void OpenWebsocket(string ipAdress, int port)
        {
            reconnectFromConnectionSceneSemaphore = new SemaphoreSlim(1);
            if(tryingToReconnect) {reconnectFromConnectionSceneSemaphore = new SemaphoreSlim(0);}
            
            activeScene = "";
            _sceneSwitcher = FindObjectOfType<SceneSwitcher>();

            _communication = new Communication(ipAdress, port);
            _readMessage = _communication.rMessage;
            _writeMessage = new WriteMessage(_communication);

            _readMessage.OnHELLO_CLIENT += OnHELLO_CLIENT;
            _readMessage.OnPARTICIPANTS_INFO += OnPARTICIPANTS_INFO;
            _readMessage.OnGAME_START += OnGAME_START;
            _readMessage.OnCHARACTER_OFFER += OnCHARACTER_OFFER;
            _readMessage.OnGAME_STATE += OnGAME_STATE;
            _readMessage.OnERROR += OnERROR;
            _readMessage.OnINVALID_MESSAGE += OnINVALID_MESSAGE;
            _readMessage.OnCARD_OFFER += OnCARD_OFFER;
            _readMessage.OnROUND_START += OnROUND_START;
            _readMessage.OnCARD_EVENT += OnCARD_EVENT;
            _readMessage.OnSHOT_EVENT += OnSHOT_EVENT;
            _readMessage.OnRIVER_EVENT += OnRIVER_EVENT;
            _readMessage.OnGAME_END += OnGAME_END;
            _readMessage.OnPAUSED += OnPAUSED;
            _readMessage.OnEAGLE_EVENT += OnEAGLE_EVENT;
            
            SceneManager.sceneLoaded += OnSceneLoaded;

            //new Thread(() => testOutput()).Start(); //TODO: Diese Zeile sollte später entfernt werden, wenn der zugehörige Test korrekt funktioniert. Das soll @Martin entfernen
        }

        /// <summary>
        /// This method is called, when the scene changes. It changes the variable activeScene to the new scene name.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            activeScene = SceneManager.GetActiveScene().name;
            Debug.Log("The scene is now: " + activeScene);
        }


        /// <summary>
        /// this method is called on HELLO_CLIENT message.
        /// The variable "_changeSceneToWaitingOnOtherPlayers" is changed so that the main thread can change the scene
        /// </summary>
        /// <param name="message"></param>
        private void OnHELLO_CLIENT(HELLO_CLIENT_Message message)
        {
            _changeSceneToWaitingOnOtherPlayers = true; //set this variable = true, to switch the scene

            StaticVariables.boardConfig = message.data.boardConfig;
            StaticVariables.gameConfig = message.data.gameConfig;
            StaticVariables.reconnectToken = message.data.reconnectToken;


            _firstHello_Client = true;
            
            Debug.Log("HELLO_CLIENT_Message");
        }
        
        /// <summary>
        /// this method is called on PARTICIPANTS_INFO message
        /// </summary>
        /// <param name="message"></param>
        private void OnPARTICIPANTS_INFO(PARTICIPANTS_INFO_Message message)
        {
            if (_firstHello_Client) //if we received the Hello_Client Message then...
            {
                //if(tryingToReconnect) participantsInfoSemaphore.Release();
                
                participantsInfoSemaphore.Wait();
                if (_firstParticipantsInfo == false) //we need this for the spectator. It can happen, that the second received Message for the client isn't a participants info!
                {
                    _firstParticipantsInfo = true;
                    StaticVariables.participantsInfoMessage = message;
                }
                
                if (activeScene.Equals("Waiting for other Players") || activeScene.Equals("Characterselection")) //is the currently viewed scene "Waiting for other Players"?
                {
                    Debug.Log("Participants for Waiting Scene or Characterselection Scene");
                    PlayerReadyScene playerReadyScene = PlayerReadyScene.Instance;
                    
                    playerReadyScene.OnParticipantsInfoMessage(message);

                    StaticVariables.participantsInfoMessage = message;
                }
                else
                {
                    if (!tryingToReconnect) _participantsInfoMessage = message;
                    /*if (tryingToReconnect)
                    {
                        _changeSceneToGame = true;
                        reconnectFromConnectionSceneSemaphore.Wait();
                        _participantsInfoMessage = message;
                        reconnectFromConnectionSceneSemaphore.Release();
                    }
                    else
                    {
                        _participantsInfoMessage = message;
                    }*/
                }

                participantsInfoSemaphore.Release();
                Debug.Log("PARTICIPANTS_INFO_Message");
            }
            else
            {
                Communication._messageQueue.Enqueue(JsonConvert.SerializeObject(message));
                new Thread(() => _communication.ReceivedMessage()).Start();
                Debug.Log("Waiting on Hello_Client Message. Enqueue Participants Info");
            }
        }

        /// <summary>
        /// this method is called on GAME_START message
        /// </summary>
        /// <param name="message"></param>
        private void OnGAME_START(GAME_START_Message message)
        {
            Debug.Log("GAME_START_Message");
        }
        
        /// <summary>
        /// this method is called on CHARACTER_OFFER message
        /// </summary>
        /// <param name="message"></param>
        private void OnCHARACTER_OFFER(CHARACTER_OFFER_Message message)
        {
            //DONE: Diese Methode implementieren! 
            CharacterSelectionUI.changeCharacters(message.data.characters[0], message.data.characters[1]);
            _changeSceneToCharacterSelection = true;
            Debug.Log("CHARACTER_OFFER_Message");
        }
        
        /// <summary>
        /// this method is called on GAME_STATE message
        /// </summary>
        /// <param name="message"></param>
        private void OnGAME_STATE(GAME_STATE_Message message)
        {
            if (_firstParticipantsInfo) //if there was no particpants info, then ignore the Game_state (can happen, when a spectator is connecting and the participants info is slower then the game state)
            {
                if (activeScene.Equals("Characterselection") || activeScene.Equals("Waiting for other Players") && StaticVariables.playerIsSpectator || tryingToReconnect) //is the currently viewed scene "Waiting for other Players"? OR is the new connecting player a spectator
                {
                    Debug.Log("Switch to Game Scene");
                    _changeSceneToGame = true; 
                }
            
                gameStateSemaphore.Wait();
                if (tryingToReconnect)
                {
                    reconnectSemephore.Wait();
                    tryingToReconnect = false;
                    //reconnectSemephore.Release();
                }
                _gameStateMessage = message;

                gameStateSemaphore.Release();
                
                Debug.Log("GAME_STATE_Message");
            }
            else
            {
                Communication._messageQueue.Enqueue(JsonConvert.SerializeObject(message));
                new Thread(() => _communication.ReceivedMessage()).Start();
                Debug.Log("Waiting on Participants_Info Message. Enqueue Game State");
            }
        }
        
        /// <summary>
        /// this method is called on ERROR message
        /// </summary>
        /// <param name="message"></param>
        private void OnERROR(ERROR_Message message)
        {
            switch (message.data.errorCode)
            {
                case 0: 
                    ErrorGameScene.errorMessage0 = message.data.reason;
                    Debug.Log(message.data.reason);
                    ConnectionScene.errorMessage0 = message.data.reason;
                    ConnectionLost.errorText = message.data.reason;
                    break;
                case 1:
                    ConnectionLost.errorText = "Error 1: Unable to connect";
                    switchToConnectionLost = true;
                    break;
                case 2:
                    ConnectionScene.errorCode2 = true;
                    break;
                case 3: 
                    ConnectionScene.errorCode3 = true;
                    break;
                case 4: 
                    ConnectionScene.errorCode4 = true;
                    break;
                case 5: 
                    ConnectionScene.errorCode5 = true;
                    break;
                case 6: 
                    errorCode6 = true;
                    break;
                case 7: 
                    errorCode7 = true;
                    break;
                case 8: 
                    ErrorGameScene.errorCode8 = true;
                    break;
                case 9: 
                    ErrorGameScene.errorCode9 = true;
                    break;
                case 10: 
                    ErrorGameScene.errorCode10 = true;
                    break;
            }
            Debug.Log("ERROR_Message");
        }
        
        /// <summary>
        /// this method is called on INVALID_MESSAGE message
        /// </summary>
        /// <param name="message"></param>
        private void OnINVALID_MESSAGE(INVALID_MESSAGE_Message message)
        {
            _invalidMessage = message;
            Debug.Log("INVALID_MESSAGE_Message");
        }
        
        /// <summary>
        /// this method is called on CARD_OFFER message
        /// </summary>
        /// <param name="message"></param>
        public void OnCARD_OFFER(CARD_OFFER_Message message)
        {
            cardOfferSemephore.Wait();
            _cardOfferMessage = message;
            cardOfferSemephore.Release();
            
            Debug.Log("CARD_OFFER_Message");
        }
        
        /// <summary>
        /// this method is called on ROUND_START message
        /// </summary>
        /// <param name="message"></param>
        private void OnROUND_START(ROUND_START_Message message)
        {
            _onRoundStart = true;
            _roundStartMessage = message;
            
            Debug.Log("ROUND_START_Message");
        }
        
        /// <summary>
        /// this method is called on CARD_EVENT message
        /// </summary>
        /// <param name="message"></param>
        private void OnCARD_EVENT(CARD_EVENT_Message message)
        {
            _cardEventMessage = message;
            Debug.Log("CARD_EVENT_Message");
        }
        
        /// <summary>
        /// this method is called on SHOT_EVENT message
        /// </summary>
        /// <param name="message"></param>
        private void OnSHOT_EVENT(SHOT_EVENT_Message message)
        {
            _shotEventMessage = message;
            Debug.Log("SHOT_EVENT_Message");
        }
        
        /// <summary>
        /// this method is called on RIVER_EVENT message
        /// </summary>
        /// <param name="message"></param>
        private void OnRIVER_EVENT(RIVER_EVENT_Message message)
        {
            _riverEventMessage = message;
            Debug.Log("RIVER_EVENT_Message");
        }
        
        /// <summary>
        /// this method is called on GAME_END message
        /// </summary>
        /// <param name="message"></param>
        private void OnGAME_END(GAME_END_Message message)
        {
            StaticVariables.gameEndMessage = message;
            _onGameEnd = true;
            /*gameEndSemaphore.Wait();
            Debug.Log("Setting GAME_END_Message");
            _gameEndMessage = message;
            gameEndSemaphore.Release();*/
            Debug.Log("GAME_END_Message");
        }
        
        /// <summary>
        /// this method is called on PAUSED message
        /// </summary>
        /// <param name="message"></param>
        private void OnPAUSED(PAUSED_Message message)
        {
            if (activeScene.Equals("Game"))
            {
                PauseButtonScript.instance.pauseCopy = !PauseButtonScript.instance.pauseCopy;
            }
            else
            {
                Communication._messageQueue.Enqueue(JsonConvert.SerializeObject(message));
                new Thread(() => _communication.ReceivedMessage()).Start();
                Debug.Log("Waiting that the Game Scene Loads. Enqueue Game State");
            }

            Debug.Log("PAUSED_Message");
        }
        
        
        /// <summary>
        /// this method is called on EAGLE_EVENT message
        /// </summary>
        /// <param name="message"></param>
        private void OnEAGLE_EVENT(EAGLE_EVENT_Message message)
        {
            _eagleEventMessage = message;
            Debug.Log("EAGLE_EVENT_Message");
        }


        /// <summary>
        /// checks if the websocket is open
        /// </summary>
        /// <returns></returns>
        public bool isWebsocketOpen()
        {
            //Debug.Log("Is Websocket open? " + _communication.isWebsocketOpen());
            return _communication.isWebsocketOpen();
        }

        /// <summary>
        /// closes the websocket
        /// </summary>
        public void closeWebsocket()
        {
            Debug.Log("closing Websocket");
            _communication.CloseWebSocketConnection();
        }


        /// <summary>
        /// Some things can only be dealt with in a main thread. The update method is a main thread.
        /// It deals with other scripts and components.
        /// </summary>
        private void Update()
        {
            if (_changeSceneToWaitingOnOtherPlayers)
            {
                _sceneSwitcher.SwitchScene("Waiting for other Players");
                _changeSceneToWaitingOnOtherPlayers = false;
                
                //create file to safe reconnect token and playername
                string infoForReconectFile = StaticVariables.playerName + "\n" + StaticVariables.reconnectToken + "\n" + StaticVariables.ip + "\n" + StaticVariables.port;
            
                string relativePath;
                if (Application.platform == RuntimePlatform.OSXPlayer)
                {
                    relativePath = "Resources/Data/StreamingAssets/";
                }
                else 
                {
                    relativePath = "StreamingAssets/";
                }
                
                string fileName = "reconnect.txt";
                string path = Path.Combine(Application.dataPath, relativePath, fileName);

                if (File.Exists(path)) //delete old file
                {
                    File.Delete(path);
                    Console.WriteLine("deleted old reconnect file");
                }
            
                //creating new file
                File.WriteAllText(path,infoForReconectFile);
                Console.WriteLine("created new reconnect file");
            }

            if (_changeSceneToCharacterSelection)
            {
                _sceneSwitcher.SwitchScene("Characterselection");
                _changeSceneToCharacterSelection = false;
            }

            if (_changeSceneToGame)
            {
                _sceneSwitcher.SwitchScene("Game");
                _changeSceneToGame = false;
            }

            if (_cardOfferMessage is not null)
            {
                foreach (var card in DisplayingCards.instance.cards)
                {
                    if (card is not null) Destroy(card); //can be null, when reconnecting and the client receives instantly a card Offer message
                }

                DisplayingCards.instance.cards = new List<GameObject>();

                DisplayingCards.instance.DisplayCards(_cardOfferMessage.data.cards.ToList());
                if (MessageSender.buttonInstance is not null) MessageSender.buttonInstance.interactable = true;
                _cardOfferMessage = null;
            }

            if (_onRoundStart)
            {
                if (!StaticVariables.playerIsSpectator)
                {
                    MessageSender.buttonInstance.interactable = false;
                    ColorBlock cb = MessageSender.buttonInstance.colors;
                    cb.normalColor = new Color(255, 255, 255, 10);
                    MessageSender.buttonInstance.colors = cb;
                }

                _onRoundStart = false;
            }

            if (_participantsInfoMessage is not null) //TODO: AB hier aufruf an den GameManager @Yannick 
            {
                if (StaticVariables.playerIsSpectator) {Debug.Log("Participants Info As Spectator");}
                GameManager.instance.onParticipants_Info(_participantsInfoMessage.data);
                _participantsInfoMessage = null;
            }
            
            if (_gameStateMessage is not null)
            {
                /*foreach (var playerState in _gameStateMessage.data.playerStates)
                {
                    Debug.Log("MessageManager: "+ playerState.playerName);
                }*/
                GameManager.instance.onGameState(_gameStateMessage);
                if (_playerStateScript is not null) _playerStateScript.StateUpdate(_gameStateMessage.data);
                //if (activeScene.Equals("Game")) GameManager.instance.onGameState(_gameStateMessage);
                _gameStateMessage = null;
            }

            if (_shotEventMessage is not null)
            {
                GameManager.instance.onShotEvent(_shotEventMessage);
                if (_playerStateScript is not null) _playerStateScript.StateUpdate(_shotEventMessage.data);
                _shotEventMessage = null;
            }
            if (_riverEventMessage is not null)
            {
                GameManager.instance.onRiverEvent(_riverEventMessage);
                if (_playerStateScript is not null) _playerStateScript.StateUpdate(_riverEventMessage.data);
                _riverEventMessage = null;
            }
            
            if (_eagleEventMessage is not null)
            {
                GameManager.instance.onEagleEvent(_eagleEventMessage);
                _eagleEventMessage = null;
            }

            if (_cardEventMessage is not null)
            {
                //if (!StaticVariables.playerIsSpectator)
                //{
                GameManager.instance.onCardEvent(_cardEventMessage);
                    if (_playerStateScript is not null) _playerStateScript.StateUpdate(_cardEventMessage.data);
                    _cardEventMessage = null;
                //}
            }

            if (_onGameEnd)
            {
                _sceneSwitcher.SwitchScene("Endscreen");
                _onGameEnd = false;
            }
            
            if (switchToConnectionLost)
            {
                //GameObject.Find("MessageManager").name = "DeleteThisGameObject";
                instance = null;
                
                var dontDestroyObjects = GameObject.Find("MessageManager");
                if (dontDestroyObjects.gameObject.scene.buildIndex == -1)
                    Destroy(dontDestroyObjects.gameObject);

                
                _sceneSwitcher.SwitchScene("Connection Lost");
                switchToConnectionLost = false;
            }

            /*if (_gameEndMessage is not null)
            {
                Debug.Log("Loaded");
                EndCard.instance.OnGAME_END(_gameEndMessage);
                _gameEndMessage = null;
            }*/

            if (errorCode6)
            { 
                if (!ConnectionLost.onInvalidMessage) ConnectionLost.errorText = "Error 6: no valid card selection.";
                ConnectionLost.onError = true;
                _sceneSwitcher.SwitchScene("Connection Lost");
                errorCode6 = false;
            }
            
            if (errorCode7)
            {
                if (!ConnectionLost.onInvalidMessage) ConnectionLost.errorText = "Error 7: no valid character selection.";
                ConnectionLost.onError = true;
                _sceneSwitcher.SwitchScene("Connection Lost");
                errorCode7 = false;
            }
            
            if (_invalidMessage is not null)
            { 
                Debug.Log("Invalid Message:\r\n" + _invalidMessage.data.invalidMessage);
                ConnectionLost.errorText = "Invalid Message - Client banned.";
                ConnectionLost.onInvalidMessage = true;
                _sceneSwitcher.SwitchScene("Connection Lost");
                _invalidMessage = null;
            }

            if (_roundStartMessage is not null)
            {
                //if (_playerStateScript is not null) _playerStateScript.StateUpdate(_roundStartMessage.data);
                _roundStartMessage = null;
            }

            if (tryingToReconnect && activeScene.Equals("Game"))
            {
                reconnectFromConnectionSceneSemaphore.Release();
                reconnectSemephore.Release();
                tryingToReconnect = false;
            } 
            
        }

        /// <summary>
        /// sets this component on DontDestroyOnLoad after it is loaded the first time
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
    }
}
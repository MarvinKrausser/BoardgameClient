using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using communication;
using entities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace managers
{
    /// <summary>
    /// This is the class that is corresponding for handeling the whole GameScene and its components. All messages relevant for the infomation in the game
    /// are used in here.
    /// </summary>
    public class GameManager : MonoBehaviour {
        public static GameManager instance;
        private Gameboard _gameboard;

        private float panelSpeed = 1.0f; 
    
        [Header("Character Sprites")]
        //all the possible character sprites
        public CharacterObject aragorn;
        public CharacterObject arwen;
        public CharacterObject boromir;
        public CharacterObject frodo;
        public CharacterObject galadriel;
        public CharacterObject gandalf;
        public CharacterObject gimli;
        public CharacterObject gollum;
        public CharacterObject legolas;
        public CharacterObject merry;
        public CharacterObject pippin;
        public CharacterObject sam;
        public CharacterObject baumbart;
    
        [Header("Player Prefab")]
        public Player playerPrefab;

        public GameObject projectilePrefab;

        private Player mainPlayer;
        /// <summary>
        /// List of all players, AIs, spectators and players that are ready.
        /// </summary>
        public List<Player> allPlayers;
        public List<Player> referencedPlayers;
        public List<Player> spectators;
        public List<Player> ais;
        public List<Player> readyPlayers;
        public int currentRound;

    
        [Header("Shot Event Variables")]
        //variables for shot message
        public TextMeshProUGUI Shot_Text;
        public RectTransform panelRectTransform;
        private bool isAnimating = false;
        private Queue<string> messageQueue = new Queue<string>();

    

        private Color originalTextColor;
        private Color originalPanelColor;

        /// <summary>
        /// Initializes all necessary instances for the GameManager class.
        /// Creates the Gameboard with the BoardConfig from the Server and calls the onParticipantsInfo to initialize all players, AIs and spectators.
        /// </summary>
        private void Start()
        {
            instance = this;
            allPlayers = new List<Player>();
            spectators = new List<Player>();
            ais = new List<Player>();
            readyPlayers = new List<Player>();
            referencedPlayers = new List<Player>();
            currentRound = 0;
        
            panelRectTransform.anchoredPosition = new Vector2(Screen.width*3, panelRectTransform.anchoredPosition.y);
            
            //get test config

            _gameboard = gameObject.GetComponent<Gameboard>();
            _gameboard.initializeTilemap(StaticVariables.boardConfig);
            
            onParticipants_Info(StaticVariables.participantsInfoMessage.data);

            MessageManager.gameStateSemaphore.Release();
        }

        /// <summary>
        /// Is called on a Participants_Info from the Server. Initializes Player,Ai or Spectator if they are new or set them to ready.
        /// The Lists can be used for further actions.
        /// </summary>
        public void onParticipants_Info(PARTICIPANTS_INFO_Message_Data info)
        {
            foreach (var player in info.players)
            {
                //if player not in array
                if (!allPlayers.Any(p => p.name.Equals(player)))
                {
                    var newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    newPlayer.Initialize(player,Role.PLAYER,3); 
                    newPlayer.Height = _gameboard.HEIGHT;
                    Debug.Log("adding Players: " + newPlayer.name);
                    allPlayers.Add(newPlayer);
                }
            }
        
            foreach (var ai in info.ais)
            {
                //if ai not in array
                if (!allPlayers.Any(p => p.name.Equals(ai)))
                {
                    var newAi = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    newAi.Initialize(ai,Role.AI, 3);
                    newAi.Height = _gameboard.HEIGHT;
                    allPlayers.Add(newAi);
                }
            }
        
            foreach (var spectator in info.spectators)
            {
                //if spectator not in array
                if (!spectators.Any(p => p.name.Equals(spectator)))
                {
                    var newSpec = new Player();
                    newSpec.Initialize(spectator, Role.SPECTATOR);
                    spectators.Add(newSpec);
                }
            }

            if (info.readyPlayers is not null)
            {
                foreach (var readyPlayer in info.readyPlayers)
                {
                    //if ready player is in list
                    if (!readyPlayers.Any(p => p.name.Equals(readyPlayer)))
                    {
                        var player = allPlayers.Find(p => p.name.Equals(readyPlayer));
                        player.ready = true;
                        readyPlayers.Add(player);
                    }
                }
            }
        }

        /// <summary>
        /// Called on a incoming GameState Message. Sets state of all players and the gameboard. 
        /// </summary>
        /// <param name="state"></param>
        public void onGameState(GAME_STATE_Message state)
        {
            referencedPlayers.Clear();
            
            var stateData = state.data;

            foreach (var playerState in stateData.playerStates)
            {
                onPlayerState(playerState);
            }
            _gameboard.onBoardState(stateData.boardState);
            currentRound = stateData.currentRound;
            
            deleteDisconnected(allPlayers.Except(referencedPlayers).ToList());
        }
        
        /// <summary>
        /// Called on a incoming Card Event. Sets state of all players and the gameboard.
        /// </summary>
        /// <param name="message"></param>
        public void onCardEvent(CARD_EVENT_Message message)
        { 
            //todo: maybe a log chat for played moves
            
            referencedPlayers.Clear();
            foreach (var multPlayerState in message.data.playerStates)
            {
                foreach (var playerState in multPlayerState)
                {
                    onPlayerState(playerState);
                }
            }
        
            foreach (var boardState in message.data.boardStates) 
            { 
                 _gameboard.onBoardState(boardState);
            }
            
            deleteDisconnected(allPlayers.Except(referencedPlayers).ToList());
        }
          
        /// <summary>
        /// Called in each message that contains a player state. Initializes players if they are null, checks if the player dies or gets revived.
        /// Moves and turns the player, set the correct character and animators.
        /// Checks if the player has reached a checkpoint and sets all values for the player. 
        /// </summary>
        /// <param name="state"></param>
        public void onPlayerState(PlayerState state)
        {
            var player = allPlayers.Find(p => p.name.Equals(state.playerName));
            
            if (player is null) { 
                Debug.Log("Player is null!!!");
                var newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                newPlayer.Initialize(state.playerName,Role.PLAYER,3); //todo set lembas to config
                newPlayer.Height = _gameboard.HEIGHT;
                Debug.Log("adding Players: " + newPlayer.name);
                allPlayers.Add(newPlayer);
                
                newPlayer.ready = true;
                readyPlayers.Add(newPlayer);

                player = newPlayer;
            }
            
            if (player.Lifes > 0 && state.lives <=0)
            {
                //player gets killed
                AddMessage(player.name + " has died");
                Debug.Log(player.name + " has died");
                player.getsKilled = true;
            }
            player.Lifes = state.lives;
            player.Lembas = state.lembasCount;
            player.suspended = state.suspended;
            player.turnOrder = state.turnOrder;
            player.spawnPosition.Set(state.spawnPosition[0],state.spawnPosition[1]);
            
            //set sprite and controller for animation 
            player.Character = getCharakterSprite(state.character);
            player.animator.runtimeAnimatorController = player.Character.overrideController;

            player.TurnPlayer(state.direction);
            player.MovePlayer(new Vector2Int(state.currentPosition[0],state.currentPosition[1]));
            
            if (player.isDead && state.lives > 0)
            {
                //revive player - make him visible again
                player.revivePlayer();
                AddMessage(player.name + " was revived");
            }

            if (player.reachedCheckpoints+1 == state.reachedCheckpoints)
            {
                //player has reached a checkpoint
                player.reachedCheckpoints = state.reachedCheckpoints;
                AddMessage(player.name + " has reached a checkpoint");
                //todo: mark checkpoint as reached
            }

            referencedPlayers.Add(player);

        }
        /// <summary>
        /// Called on a shot event. Initialises the shoot animation and sets the states of the player.
        /// </summary>
        /// <param name="message"></param>
        public void onShotEvent (SHOT_EVENT_Message message)
        {
            
            referencedPlayers.Clear();

            var shooter = allPlayers.Find(p => p.name.Equals(message.data.shooterName));
            var target = allPlayers.Find(p => p.name.Equals(message.data.targetName));
        
            AddMessage(shooter.name +" has shot " + target.name);
            //the player class handles the shot animation etc. 
            shooter.shoot(target);
            //start animation for shot event in Game 
            //adds message to queue which then gets executed
            AddMessage(shooter.name+ " has shot " + target.name);
            
            foreach (var playerState in message.data.playerStates)
            {
                onPlayerState(playerState);
            }
            
            deleteDisconnected(allPlayers.Except(referencedPlayers).ToList());

            Debug.Log(shooter.name +"has shot " + target.name);
        }
        
        /// <summary>
        /// Called on a river event. Sets all player states and the boardstate.
        /// </summary>
        /// <param name="message"></param>
        public void onRiverEvent(RIVER_EVENT_Message message)
        {
            referencedPlayers.Clear();
            AddMessage(message.data.playerName + " is moved by river");
            
            foreach (var multPlayerStates in message.data.playerStates)
            {
                foreach (var playerState in multPlayerStates)
                {
                    onPlayerState(playerState);
                }
            }
            deleteDisconnected(allPlayers.Except(referencedPlayers).ToList());

            foreach (var boardState in message.data.boardStates)
            {
                _gameboard.onBoardState(boardState);
            }
        }

        /// <summary>
        /// Adds a message to the queue that triggers the information banner.
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            messageQueue.Enqueue(message);
            if (!isAnimating)
            {
                StartCoroutine(showPanel());
            }
        }
        /// <summary>
        /// Animates a Panes / Banner that flies in from the right, displays a current information and flies out to the left.
        /// This is called on any major event like the card event, river event, player death / revive and shot event.
        /// </summary>
        /// <returns></returns>
        IEnumerator showPanel()
        {
            if (isAnimating || messageQueue.Count == 0)
            {
                yield break;
            }

            isAnimating = true; // We are starting the animation

            string message = messageQueue.Dequeue();
            Shot_Text.text = message;

            Vector2 startPosition = new Vector2(Screen.width * 3, panelRectTransform.anchoredPosition.y);
            Vector2 middlePos1 = new Vector2(10, panelRectTransform.anchoredPosition.y);
            Vector2 middlePos2 = new Vector2(-10, panelRectTransform.anchoredPosition.y);
            Vector2 endPosition = new Vector2(-Screen.width * 3, panelRectTransform.anchoredPosition.y);

            float fastSpeed = panelSpeed * 0.1f;
            float slowSpeed = panelSpeed;
            float elapsedTime;

            // Fly in fast
            elapsedTime = 0;
            while (elapsedTime < fastSpeed)
            {
                panelRectTransform.anchoredPosition = Vector2.Lerp(startPosition, middlePos1, (elapsedTime / fastSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Move in slow motion
            elapsedTime = 0;
            while (elapsedTime < slowSpeed)
            {
                panelRectTransform.anchoredPosition = Vector2.Lerp(middlePos1, middlePos2, (elapsedTime / slowSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Fly out fast
            elapsedTime = 0;
            while (elapsedTime < fastSpeed)
            {
                panelRectTransform.anchoredPosition = Vector2.Lerp(middlePos2, endPosition, (elapsedTime / fastSpeed));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the panel ends at the exact off-screen position
            panelRectTransform.anchoredPosition = endPosition;

            isAnimating = false; // Animation is done

            if (messageQueue.Count > 0) // If there are more messages in the queue
            {
                StartCoroutine(showPanel()); // Start the next animation
            }
        }

        public void onEagleEvent(EAGLE_EVENT_Message message)
        {
            AddMessage(message.data.playerName + " is free like an Adler");

            foreach (var playerState in message.data.playerStates)
            {
                if (message.data.playerName.Equals(playerState.playerName))
                {
                    var player = allPlayers.Find(p => p.name.Equals(playerState.playerName));
                    player.teleportPlayer(new Vector3(playerState.currentPosition[0],playerState.currentPosition[1]));
                }
                onPlayerState(playerState);
            }
        }
        
        private CharacterObject getCharakterSprite(Character character)
        {
            switch (character)
            {
                case Character.FRODO:
                    return frodo;
                case Character.SAM:
                    return sam;
                case Character.LEGOLAS:
                    return legolas;
                case Character.GIMLI:
                    return gimli;
                case Character.GANDALF:
                    return gandalf;
                case Character.ARAGORN:
                    return aragorn;
                case Character.GOLLUM:
                    return gollum;
                case Character.GALADRIEL:
                    return galadriel;
                case Character.BOROMIR:
                    return boromir;
                case Character.BAUMBART:
                    return baumbart;
                case Character.MERRY:
                    return merry;
                case Character.PIPPIN:
                    return pippin;
                case Character.ARWEN:
                    return arwen;
                default:
                    throw new ArgumentOutOfRangeException(nameof(character), character, null);
            }
        }

        /// <summary>
        /// Deletes all players that are displayed on the gameboard but were not referenced in the last player states, which means they have disconnected.
        /// </summary>
        /// <param name="missingPlayerList"></param>
        private void deleteDisconnected(List<Player> missingPlayerList)
        {
            foreach (var player in missingPlayerList)
            {
                if (player != null)
                {
                    allPlayers.Remove(player);
                    Destroy(player.gameObject);
                }
                else
                {
                    Debug.Log("Player could not be destroyed since it is already null");
                }
            }
        }
    }
}
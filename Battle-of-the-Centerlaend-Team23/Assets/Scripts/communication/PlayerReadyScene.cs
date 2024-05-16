using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using communication;
using managers;
//using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// Representative for the PlayerReadyScene. Updates text of all players that are ready as well as the own ready status.
/// </summary>
public class PlayerReadyScene : MonoBehaviour
{
    public static PlayerReadyScene Instance { get; set; }

    public static Role playerRole;
    
    //TODO: Erstelle eine Queue (oder etwas ähnliches) um die Participant Infos dort reinzuspeichern. Dafür soll in der Communication "Thread.Sleep(100)" entfernt werden
    
    [SerializeField] private TMP_Text _readyPlayerList;
    [SerializeField] private TMP_Text _viewerPlayerList;
    [SerializeField] private Button _readyButton;

    private TextMeshProUGUI readyButtonText;
    
    private MessageManager _messageManager;
    private bool _changeTextInput;

    private string _readyPlayersString;
    private string _unreadyPlayersString;
    private string _spectatorsString;

    /// <summary>
    /// If the ready button is clicked the respective message is sent and the button UI gets updated
    /// </summary>
    public void OnButtonReady()
    {
        string currentText = readyButtonText.text; //getting the text of the TextMeshProUGUI Component
        
        if (currentText.Equals("Bereit"))
        {
            readyButtonText.text = "Nicht Bereit";
            _messageManager._writeMessage.WriteMessagePLAYER_READY(true);
        }
        else
        {
            readyButtonText.text = "Bereit";
            _messageManager._writeMessage.WriteMessagePLAYER_READY(false);
        }  
    }

    /// <summary>
    /// Updates the UI for each incoming Participants Information from the server.
    /// </summary>
    /// <param name="message"></param>
    public void OnParticipantsInfoMessage(PARTICIPANTS_INFO_Message message)
    {
        //"??" is testing, if an array is not null or is null. If its null, then Enumerable.Empty<string>() will be choosed insteed of null.
        _readyPlayersString = "";
        _unreadyPlayersString = "";
        _spectatorsString = "";
        string[] players = message.data.players;
        string[] ais = message.data.ais;
        string[] participants;
        if (players != null && ais != null)
        {
            participants = players.Concat(ais).ToArray();
        } 
        else if (players != null)
        {
            participants = players;
        }
        else //this case can usually not happen, because this client is a "player" and should be in the players array
        {
            participants = ais;
        }
        
        string[] readyPlayers = message.data.readyPlayers;
        if (readyPlayers == null) readyPlayers = new [] {""};
        
        string[] unreadyPlayers = participants.Except(readyPlayers).ToArray();
        
        string[] spectators = message.data.spectators;
        if (spectators == null) spectators = new [] {""};

        for (int i = 0; i < readyPlayers.Length; i++) //create a _readyPlayerString with all ready players in it
        {
            _readyPlayersString += readyPlayers[i] + "\n";
            //if (i != readyPlayers.Length - 1) _readyPlayersString += "\n";
        }
        for (int i = 0; i < unreadyPlayers.Length; i++) //create a _readyPlayerString with all unready players in it
        {
            _unreadyPlayersString += unreadyPlayers[i] + "\n";
        }
        for (int i = 0; i < spectators.Length; i++) //create a _readyPlayerString with all spectators players in it
        {
            _spectatorsString += spectators[i] + "\n";
        }

        _changeTextInput = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerRole.Equals(Role.PLAYER))
        {
            readyButtonText = _readyButton.GetComponentInChildren<TextMeshProUGUI>(); //getting the TextMeshProUGUI Component from Button
            readyButtonText.text = "Bereit";
        }
        else
        {
            _readyButton.gameObject.SetActive(false);
        }
        
        Instance = this;
        
        _changeTextInput = false;

        _messageManager = MessageManager.instance;
        
        MessageManager.participantsInfoSemaphore.Release();
    }

    // Update is called once per frame
    void Update()
    {
        if (_changeTextInput)
        {
            _readyPlayerList.text = string.Format("<color=green>{0}</color><color=red>{1}</color>", _readyPlayersString, _unreadyPlayersString);
            _viewerPlayerList.color = Color.gray;
            _viewerPlayerList.text = _spectatorsString;
            _changeTextInput = false;
        }
    }
}

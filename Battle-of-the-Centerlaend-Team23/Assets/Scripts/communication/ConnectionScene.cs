using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using communication;
using managers;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Debug = UnityEngine.Debug;
/// <summary>
/// Representative class for the ConnectScene. Uses the input from the user to set all variables and create a connection to the server.
/// </summary>
public class ConnectionScene : MonoBehaviour
{
    [SerializeField] private TMP_InputField _ip;
    [SerializeField] private TMP_InputField _port;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private TMP_Dropdown _playerOrSpectator;
    [SerializeField] private Button _connectButton;
    [SerializeField] private TMP_Text _connectingText;
    [SerializeField] private Button _reconnectButton;
    [SerializeField] private Button _mainMenuButton;

    private MessageManager _messageManager;
    
    private readonly int defaultPort = 3018;
    private readonly Role defaultplayerOrSpectator = Role.PLAYER;

    public bool defaultValues;

    private bool _connection;
    private bool _ipCorrect;
    private bool _portCorrect;
    private bool _playerNameCorrect;

    private bool connectionFailedBool;

    public static string errorMessage0;

    public static bool errorCode2;
    public static bool errorCode3;
    public static bool errorCode4;
    public static bool errorCode5;

    private string ip;
    private int port;
    private string playerName;
    private Role playerRole;

    /// <summary>
    /// The data is read from the "reconnect.txt" file (if it exists). An attempt is then made to restore the connection
    /// </summary>
    public void ReconnectButton()
    {
        interactWithComponents(false);
        
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

        if (File.Exists(path)) //file not exists
        {
            _connectingText.color = Color.green;
            _connectingText.text = "Reconnecting...";
            
            string[] reconnectLines = File.ReadAllLines(path); //0 == playername && 1 == reconnect-token && 2 == ip && 3 == port
            if (reconnectLines.Length == 4)
            {
                _messageManager = FindObjectOfType<MessageManager>();
                _messageManager.tryingToReconnect = true;
                _messageManager.OpenWebsocket(reconnectLines[2], int.Parse(reconnectLines[3]));
                MessageManager.connectionAttempts++;
                
                DateTime startTime = DateTime.Now;
                TimeSpan desiredDuration = TimeSpan.FromSeconds(3);
                while (DateTime.Now - startTime < desiredDuration) //busy wait (=aktives Warten) for 3 seconds if the Client can't connected to Server. //TODO: Könnte man das hier noch schöner machen?
                {
                    if (_messageManager.isWebsocketOpen())
                    {
                        break;
                    }
                }
                
                if (_messageManager.isWebsocketOpen()) //can connect if there is no error!
                {
                    StaticVariables.ip = reconnectLines[2];
                    ip = reconnectLines[2];
                    StaticVariables.port = int.Parse(reconnectLines[3]);
                    port = int.Parse(reconnectLines[3]);
                    StaticVariables.playerName = reconnectLines[0];
                    playerName = reconnectLines[0];
            
                    MessageManager.instance = _messageManager;
                    _messageManager._writeMessage.WriteMessageRECONNECT(reconnectLines[0], Guid.Parse(reconnectLines[1]));
                    StaticVariables.playerIsSpectator = playerRole.Equals(Role.SPECTATOR);
                    Debug.Log("reconnected to Server. IP = " + ip + " and port = " + port + ". Your name = " + playerName);

                    new Thread(() => connectionFailed()).Start();
                }
                else
                {
                    _messageManager.tryingToReconnect = false;
                    
                    _connectingText.text = "Can't reconnect";
                    _connectingText.color = Color.red;
                    Debug.Log("No connection to Server. IP = " + ip + " and port = " + port + ". Your name = " + playerName);
                    _ip.textComponent.color = Color.red;
                    _port.textComponent.color = Color.red;
                
                    interactWithComponents(true);
                }
            }
            Console.WriteLine("trying to reconnect");
        }
        else
        {
            _connectingText.color = Color.red;
            _connectingText.text = "can't reconnect";
            interactWithComponents(true);
        }
        
        interactWithComponents(true);
    }
    
    /// <summary>
    /// Reads in the information from the user, checks if they are valid and calls the connect to the server coroutine.
    /// This Method is called when the client wants to connect to the server. Tries to connect to the given IP and port with the name and playerrole.
    /// if it works, then the connection is established. If not, then the client can connect again.
    /// </summary>
    public void ConnectButton()
    {
        interactWithComponents(false);

        _connection = false;
        _ipCorrect = _portCorrect = _playerNameCorrect = true;
        _messageManager = FindObjectOfType<MessageManager>();
        _messageManager.tryingToReconnect = false;
        
        //Take the text content from input fields and dropdown
        ip = _ip.text.Trim().Replace(" ", "");
        string portString = _port.text.Trim().Replace(" ", "");
        playerName = _playerName.text.Trim();
        int selectedPlayerOrSpectator = _playerOrSpectator.value; //if selectedPlayerOrSpectator == 0 then "player" else (if selectedPlayerOrSpectator == 1 then) "spectator"

        if (defaultValues)
        {
            ip = "127.0.0.1";
            portString = 3018.ToString();
            playerName = "Team-23 Benutzer";

            defaultValues = false;
        }
        
        _ipCorrect = ipCheck(ip); //check
        //Debug.Log("_ipCorrect: " + _ipCorrect);

        _portCorrect = portCheck(portString); //check
        //Debug.Log("_portCorrect: " + _portCorrect);
        
        if (_portCorrect)
        {
            port = int.Parse(portString);
        }

        _playerNameCorrect = playerNameCheck(playerName); //check
        //Debug.Log("_playerNameCorrect: " + _playerNameCorrect);
        
        playerRole = defaultplayerOrSpectator;
        PlayerReadyScene.playerRole = Role.PLAYER;
        if (selectedPlayerOrSpectator == 1)
        {
            playerRole = Role.SPECTATOR;
            PlayerReadyScene.playerRole = Role.SPECTATOR;
        }
        
        
        if (_ipCorrect && _portCorrect && _playerNameCorrect) //if input is correct, try to open a Websocket
        {
            _connectingText.color = Color.green;
            _connectingText.text = "Connecting...";
            StartCoroutine(ConnectToServer());
        }
        else //if input is wrong, then stay in scene
        {
            if (!_ipCorrect) _ip.textComponent.color = Color.red;
            if (!_portCorrect) _ip.textComponent.color = Color.red;
            if (!_playerNameCorrect) _playerName.textComponent.color = Color.red;
            
            interactWithComponents(true);
            
        }
    }

    /// <summary>
    /// this method is called by the ConnectButton to connect to the server
    /// </summary>
    /// <returns></returns>
    IEnumerator ConnectToServer()
    {
        yield return null;
        Communication._messageQueue = new Queue<string>();
        _messageManager.OpenWebsocket(ip,port);
        MessageManager.connectionAttempts++; 
        //Debug.Log("increment. New Value: " + MessageManager.connectionAttempts);
        DateTime startTime = DateTime.Now;
        TimeSpan desiredDuration = TimeSpan.FromSeconds(3);
        while (DateTime.Now - startTime < desiredDuration) //busy wait (=aktives Warten) for 3 seconds if the Client can't connected to Server. //TODO: Könnte man das hier noch schöner machen?
        {
            if (_messageManager.isWebsocketOpen())
            {
                break;
            }
        }
        if (_messageManager.isWebsocketOpen()) //can connect if there is no error!
        {
            StaticVariables.ip = ip;
            StaticVariables.port = port;
            StaticVariables.playerName = playerName;
            
            MessageManager.instance = _messageManager;
            _messageManager._writeMessage.WriteMessageHELLO_SERVER(playerName, playerRole);
            StaticVariables.playerIsSpectator = playerRole.Equals(Role.SPECTATOR);
            Debug.Log("connected to Server. IP = " + ip + " and port = " + port + ". Your name = " + playerName);

            new Thread(() => connectionFailed()).Start();
        }
        else
        {
            _connectingText.text = "Connection failed";
            _connectingText.color = Color.red;
            Debug.Log("No connection to Server. IP = " + ip + " and port = " + port + ". Your name = " + playerName);
            _ip.textComponent.color = Color.red;
            _port.textComponent.color = Color.red;
                
            interactWithComponents(true);
        }
        
        yield return null;
    }

    /// <summary>
    /// helper method, to reset text in a screen
    /// </summary>
    private void connectionFailed()
    {
        Thread.Sleep(3000);
        connectionFailedBool = true;
    }

    /// <summary>
    /// enabling or disabling the buttons and input fields and dropdown menu
    /// </summary>
    /// <param name="enOrDis"></param>
    private void interactWithComponents(bool enOrDis)
    {
        _ip.enabled  = enOrDis;
        _port.enabled  = enOrDis;
        _playerName.enabled  = enOrDis;
        _playerOrSpectator.enabled  = enOrDis;
        _connectButton.enabled  = enOrDis;
        _reconnectButton.enabled  = enOrDis;
        _mainMenuButton.enabled = enOrDis;
    }
    
    /// <summary>
    /// checks if the IP Adreess length is > 0
    /// </summary>
    /// <param name="ip"></param>
    /// <returns></returns>
    private bool ipCheck(string ip)
    {
        if (ip.Length > 0) //checking if there is an input
        {
            return true;
        }
        return false;
        
    }

    /// <summary>
    /// checks if the port is a number between 1024 and 65535
    /// </summary>
    /// <param name="portString"></param>
    /// <returns></returns>
    private bool portCheck(string portString) 
    {
        int port;
        
        if (portString.Length > 0) //checking if there is an input
        {
            try
            {
                port = int.Parse(portString);
                
                if (port > 1024 && port < 65535) //check if it is a correct port number
                {
                    return true;
                }

                return false;
            }
            catch (FormatException e)
            {
                Debug.Log("This is not a port number. Port Number is now: " + defaultPort);
                return false;
            }
        }
        
        return false;
        }

    /// <summary>
    /// Checking if the player name length is max 20. If not return false
    /// Also checking, if there is an input
    /// </summary>
    /// <param name="playerName"></param>
    /// <returns></returns>
    private bool playerNameCheck(string playerName) 
    {
        if (playerName.Length > 0 && playerName.Length < 21) //checking if there is an input
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// sets color of text fields and resets the _connectingText.text
    /// </summary>
    /// <param name="value"></param>
    private void OnIPInputFieldSelect(string value)
    {
        _connectingText.color = Color.green;
        _connectingText.text = "";
        _ip.textComponent.color = Color.black;
    }

    /// <summary>
    /// sets color of text fields and resets the _connectingText.text
    /// </summary>
    /// <param name="value"></param>
    private void OnPortInputFieldSelect(string value)
    {
        _connectingText.color = Color.green;
        _connectingText.text = "";
        _port.textComponent.color = Color.black;
    }
    
    /// <summary>
    /// sets color of text fields and resets the _connectingText.text
    /// </summary>
    /// <param name="value"></param>
    private void OnPlayerNameInputFieldSelect(string value)
    {
        _connectingText.color = Color.green;
        _connectingText.text = "";
        _playerName.textComponent.color = Color.black;
    }

    /// <summary>
    /// on Scene load, this method sets the default values
    /// </summary>
    private void Start()
    {
        defaultValues = false;
        
        _connectingText.text = "";
        _connectingText.color = Color.green;
        
        errorCode2 = false;
        errorCode3 = false;
        errorCode4 = false;
        errorCode5 = false;
        interactWithComponents(true);
        _port.text = "3018";
        
        //Add on click listener on InputFields
        _ip.onSelect.AddListener(OnIPInputFieldSelect);
        _port.onSelect.AddListener(OnPortInputFieldSelect);
        _playerName.onSelect.AddListener(OnPlayerNameInputFieldSelect);

        MessageManager.connectionAttempts = 0;
        //Debug.Log("reset. New Value: " + MessageManager.connectionAttempts);
    }
    
    /// <summary>
    /// some things can only be called by a main thread.
    /// if there is an error, the textfield should be updated.
    /// </summary>
    private void Update()
    {
        if (errorCode2)
        {
            _connectingText.color = Color.red;
            _connectingText.text = "Error 2: Name is already taken";
            interactWithComponents(true);
            errorCode2 = false;
        }
        if (errorCode3)
        {
            _connectingText.color = Color.red;
            _connectingText.text = "Error 3: Name is too long";
            interactWithComponents(true);
            errorCode3 = false;
        }
        if (errorCode4)
        {
            _connectingText.color = Color.red;
            _connectingText.text = "Error 4: Game has already started";
            interactWithComponents(true);
            errorCode4 = false;
        }
        if (errorCode5)
        {
            _connectingText.color = Color.red;
            _connectingText.text = "Error 5: The maximum number of players is already in the game";
            interactWithComponents(true);
            errorCode5 = false;
        }

        if (connectionFailedBool)
        {
            if (_connectingText.text.Equals("Connecting...") || _connectingText.text.Equals("Reconnecting..."))
            {
                _connectingText.color = Color.red;
                _connectingText.text = "Connected with Error";
                interactWithComponents(true);
            }
            connectionFailedBool = false;
        }
        
        if (errorMessage0 is not null)
        {
            _connectingText.color = Color.red;
            _connectingText.text = errorMessage0;
            interactWithComponents(true);
            errorMessage0 = null;
        }
    }
}

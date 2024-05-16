using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using communication;
using managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Representative for the Reconnect scene. Handles the behaviour if the client tries to reconnect to the server
/// </summary>
public class ConnectionLost : MonoBehaviour
{
    
    [SerializeField] private TMP_Text _disconnectText;
    [SerializeField] private GameObject _reconnectButton;

    public static string errorText;
    public static bool onError;
    public static bool onInvalidMessage;

    /// <summary>
    /// Tries to reconnect to the server. Handles all necessary operations that are required to reconnect
    /// </summary>
    public void Reconnect()
    {
        _disconnectText.color = Color.red;
        _disconnectText.text = "Can't reconnect";

        MessageManager.instance = FindObjectOfType<MessageManager>();
        MessageManager.reconnectSemephore = new SemaphoreSlim(0);

        MessageManager.gameStateSemaphore = new SemaphoreSlim(0);
        MessageManager.participantsInfoSemaphore = new SemaphoreSlim(0);
        MessageManager.reconnectSemephore = new SemaphoreSlim(0);
        MessageManager.cardOfferSemephore = new SemaphoreSlim(0);
        MessageManager.reconnectFromConnectionSceneSemaphore = new SemaphoreSlim(0);
        
        MessageManager.activeScene = "Connection lost";
        MessageManager.instance.tryingToReconnect = true;
        MessageManager.instance.OpenWebsocket(StaticVariables.ip, StaticVariables.port);
        MessageManager.connectionAttempts++;
        Thread.Sleep(1000);
        
        MessageManager.instance._writeMessage.WriteMessageRECONNECT(StaticVariables.playerName, StaticVariables.reconnectToken);
    }

    /// <summary>
    /// Displays the error that led to the change to this scene
    /// </summary>
    void Start()
    {
        _disconnectText.color = Color.red;
        if (errorText is not null && !errorText.Equals(""))
        {
            _disconnectText.text = errorText;
            errorText = "";
        }
        else
        {
            _disconnectText.text = "No error text";
        }

        if (onError || onInvalidMessage)
        {
            _reconnectButton.SetActive(false);
        }
    }
}

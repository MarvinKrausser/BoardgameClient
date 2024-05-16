using System;
using System.Collections;
using System.Collections.Generic;
using communication;
using managers;
using TMPro;
using UnityEngine;
/// <summary>
/// Handles the pause request and the respective actions
/// </summary>
public class PauseButtonScript : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI buttonText;
    public static PauseButtonScript instance;
    public bool paused { set; get; }
    [NonSerialized] public bool pauseCopy;

    private void ChangePauseStatus()
    {
        paused = !paused;
        if (paused) buttonText.text = "Resume";
        else buttonText.text = "Pause";
    }

    /// <summary>
    /// Sends a pause message to the server
    /// </summary>
    public void SendPauseMessage()
    {
        MessageManager.instance._writeMessage.WriteMessagePAUSE_REQUEST(!paused);
    }

    private void Awake()
    {
        buttonText.text = "Pause";
        if (instance is not null && instance.paused)
        {
            ChangePauseStatus();
            pauseCopy = true;
        }
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseCopy != paused) ChangePauseStatus();
    }
}

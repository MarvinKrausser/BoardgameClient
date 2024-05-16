using System.Collections;
using System.Collections.Generic;
using System.Threading;
using communication;
using managers;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Handles the behaviour of the MessageManager if it needs to be reset. For instance if you want to reconnect. 
/// </summary>
public class ResetManager : MonoBehaviour
{
    
    /// <summary>
    /// This Method resets all things, so we can restart the game
    /// </summary>
    public void ResetGame()
    {
        if (MessageManager.instance.isWebsocketOpen()) //is websocket still open, then close it.
        {
            MessageManager.instance.closeWebsocket();
        }

        ConnectionLost.onError = false;
        ConnectionLost.onInvalidMessage = false;
        MessageManager.instance = null;
        MessageManager.gameStateSemaphore = new SemaphoreSlim(0);
        MessageManager.participantsInfoSemaphore = new SemaphoreSlim(0);
        MessageManager.reconnectSemephore = new SemaphoreSlim(0);
        MessageManager.cardOfferSemephore = new SemaphoreSlim(0);
        MessageManager.reconnectFromConnectionSceneSemaphore = new SemaphoreSlim(0);
        
        DisplayingCards.instance = null;
        MessageSender.buttonInstance = null;
        Communication._messageQueue = new Queue<string>();
        PlayerReadyScene.Instance = null;
        ReadMessage.reset_schemas();
        staticVariablesReset();
        EndCard.instance = null;
        GameManager.instance = null;
        MessageManager.connectionAttempts = 0;
        MessageManager.activeScene = null;
        MessageManager._playerStateScript = null;
        MessageManager.switchToConnectionLost = false;
        ConnectionScene.errorCode2 = false;
        ConnectionScene.errorCode3 = false;
        ConnectionScene.errorCode4 = false;
        ConnectionScene.errorCode5 = false;
        ConnectionScene.errorMessage0 = null;
        PauseButtonScript.instance = null;

        //Debug.Log("reset. New Value: " + MessageManager.connectionAttempts);

        CharacterSelectionUI.instance = null;
        CharacterSelectionUI.currentCharacter = null;
        //CharacterSelectionUI.characterForSelect = null; //brauchen wir das hier überhaupt, denn auf das Array wird ja nie zugegriffen
        
        
        var dontDestroyObjects = GameObject.FindObjectsOfType<MonoBehaviour>();
        foreach (var obj in dontDestroyObjects)
        {
            if (obj.gameObject.scene.buildIndex == -1)
                Destroy(obj.gameObject);
        }

        // Lade die Startszene neu
        SceneManager.LoadScene("Startscreen"); //oder 0
        
        
    }
    
    private void staticVariablesReset()
    {
        StaticVariables.playerName = null;
        StaticVariables.boardConfig = null;
        StaticVariables.gameConfig = null;
        StaticVariables.participantsInfoMessage = null;
        StaticVariables.gameEndMessage = null;
    }


    public void DisconnectButton()
    {
        MessageManager.instance._writeMessage.WriteMessageGOODBYE_SERVER();
        
        Thread.Sleep(100);

        ResetGame();
    }


    /// <summary>
    /// this Method resets all things, when the connection is already closed or closes automatically
    /// </summary>
    public void DisconnectButtonConnectionScene()
    {
        ConnectionLost.onError = false;
        ConnectionLost.onInvalidMessage = false;
        MessageManager.instance = null;
        
        MessageManager.gameStateSemaphore = new SemaphoreSlim(0);
        MessageManager.participantsInfoSemaphore = new SemaphoreSlim(0);
        MessageManager.reconnectSemephore = new SemaphoreSlim(0);
        MessageManager.cardOfferSemephore = new SemaphoreSlim(0);
        MessageManager.reconnectFromConnectionSceneSemaphore = new SemaphoreSlim(0);
        
        DisplayingCards.instance = null;
        MessageSender.buttonInstance = null;
        Communication._messageQueue = new Queue<string>();
        PlayerReadyScene.Instance = null;
        ReadMessage.reset_schemas();
        staticVariablesReset();
        EndCard.instance = null;
        GameManager.instance = null;
        MessageManager.connectionAttempts = 0;
        MessageManager.activeScene = null;
        MessageManager._playerStateScript = null;
        MessageManager.switchToConnectionLost = false;
        ConnectionScene.errorCode2 = false;
        ConnectionScene.errorCode3 = false;
        ConnectionScene.errorCode4 = false;
        ConnectionScene.errorCode5 = false;
        ConnectionScene.errorMessage0 = null;
        PauseButtonScript.instance = null;
        //Debug.Log("reset. New Value: " + MessageManager.connectionAttempts);

        CharacterSelectionUI.instance = null;
        CharacterSelectionUI.currentCharacter = null;
        //CharacterSelectionUI.characterForSelect = null; //brauchen wir das hier überhaupt, denn auf das Array wird ja nie zugegriffen
        
        
        var dontDestroyObjects = GameObject.FindObjectsOfType<MonoBehaviour>();
        foreach (var obj in dontDestroyObjects)
        {
            if (obj.gameObject.scene.buildIndex == -1)
                Destroy(obj.gameObject);
        }

        // Lade die Startszene neu
        SceneManager.LoadScene("Startscreen"); //oder 0
    }

    public void ConnectionLostResetAll()
    {
        DisconnectButtonConnectionScene();
    }
    
}

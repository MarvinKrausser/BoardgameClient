using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using communication;
using managers;
using UnityEngine.SceneManagement;

/// <summary>
/// Displays the EndCard on the EndScene. Also lets you quit the application
/// </summary>
public class EndCard : MonoBehaviour
{
    public static EndCard instance;
    
    public TextMeshProUGUI winnerText;
    public Button mainMenuButton;
    public Button exitButton;


    private void Start()
    {
        instance = this;
        Debug.Log("EndCard started!");
        //MessageManager.gameEndSemaphore.Release();
        OnGAME_END(StaticVariables.gameEndMessage);
    }
    
    /// <summary>
    /// The application gets quit
    /// </summary>
    public void ExitGame()
    {
        Debug.Log("Quitting Game..."); // Gibt eine Debug-Nachricht aus
        Application.Quit(); // Beendet die Anwendung
    }

    /// <summary>
    /// Updates the UI to display the winner from the Game_End message
    /// </summary>
    /// <param name="message"></param>
    public void OnGAME_END(GAME_END_Message message)
    {
        Debug.Log("GAME_END_Message");

        // Spielergebnisse anzeigen
        string winner = message.data.winner;

        // Gewinner anzeigen
        DisplayWinner(winner);
    }
    

    private void DisplayWinner(string winner)
    {
        // Gewinner im TextMeshPro-Feld anzeigen
        winnerText.text = "Gewinner: " + winner;
        Debug.Log("Gewinner: " + winner);
    }
}


   


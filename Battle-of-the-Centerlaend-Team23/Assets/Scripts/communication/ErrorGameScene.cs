using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles error messages that could occur in the GameScene and displays them.
/// </summary>
public class ErrorGameScene : MonoBehaviour
{
    [SerializeField] private TMP_Text _ErrorText;
    public static string errorMessage0;
    
    public static bool errorCode8;
    public static bool errorCode9;
    public static bool errorCode10;
    
    private void OnErrorText(string text)
    {
        _ErrorText.text = text;
        StartCoroutine(WaitAndDoSomething(8f));
    }
    
    /// <summary>
    /// Coroutine that waits for the specified time and then resets the text field input.
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator WaitAndDoSomething(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        _ErrorText.text = "";
    }

    void Start()
    {
        errorMessage0 = null;
        errorCode8 = false;
        errorCode9 = false;
        errorCode10 = false;
        
        errorMessage0 = null;
        _ErrorText.color = Color.red;
        _ErrorText.text = "";
    }


    private void Update()
    {
        if (errorMessage0 is not null)
        {
            OnErrorText(errorMessage0);
            errorMessage0 = null;
        }
        
        if (errorCode8)
        {
            OnErrorText("Error 8: Kartenauswahltimeout");
            errorCode8 = false;
        }
        if (errorCode9)
        {
            OnErrorText("Error 9: Charakterauswahltimeout");
            errorCode9 = false;
        }
        if (errorCode10)
        {
            OnErrorText("Error 10: Partie ist pausiert");
            errorCode10 = false;
        }
    }
}

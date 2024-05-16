using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads, exits or switches the scene.
/// </summary>
public class SceneSwitcher : MonoBehaviour
{
   
    /*/public GameObject optionsScreen; //TODO: wozu brauchen wir das?/*/

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void exitGame()
    {
        Application.Quit();
    }
    
    /// <summary>
    /// this method should be called to switch the scene(with the scene name )
    /// </summary>
    /// <param name="scene"></param>
    public void SwitchScene(string scene)
    {
        Debug.Log("Switch Scene to: "+ scene);
        SceneManager.LoadScene(scene);
    }
    
    

}
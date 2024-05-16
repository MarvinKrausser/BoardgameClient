using communication;
using UnityEngine;

namespace managers
{
    /// <summary>
    /// Hides the pause button and the card selection UI in the game if you are a spectator.
    /// Talking about equal rights...straight back to the 60s.
    /// </summary>
    public class SpectatorManager : MonoBehaviour
    {
        [SerializeField] private GameObject _cardsInGameScene;
        [SerializeField] private GameObject _pauseButton;
        
        
        
        void Start()
        {
            if (StaticVariables.playerIsSpectator)
            {
                _cardsInGameScene.SetActive(false);
                _pauseButton.SetActive(false);
            }
        }
    }
}
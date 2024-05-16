using System.Collections;
using System.IO;
using communication;
using managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class GameManagerPlayModeTest
    {
        [UnityTest]
        public IEnumerator initializeGameTest()
        {
            var boardConfig = JsonObjects.getBoardConfig();
            var participantsInfo = JsonObjects.getParticipantsInfo();

            StaticVariables.boardConfig = boardConfig;
            StaticVariables.participantsInfoMessage = participantsInfo;
            
            
            SceneManager.LoadScene("Scenes/Game");
            yield return null;
            var gameManager = GameObject.Find("_manager").GetComponent<GameManager>();
            
            Assert.NotNull(gameManager.allPlayers);
            Assert.NotNull(gameManager.ais);
            Assert.NotNull(gameManager.readyPlayers);
            Assert.NotNull(gameManager.spectators);
            
        }
        
        [UnityTest]
        public IEnumerator onGameStateTest()
        {
            var boardConfig = JsonObjects.getBoardConfig();
            var participantsInfo = JsonObjects.getParticipantsInfo();

            StaticVariables.boardConfig = boardConfig;
            StaticVariables.participantsInfoMessage = participantsInfo;
            
            
            SceneManager.LoadScene("Scenes/Game");
            yield return null;
            var gameManager = GameObject.Find("_manager").GetComponent<GameManager>();
            
            gameManager.onGameState(JsonObjects.getGameState());
            
        }
        
        [UnityTest]
        public IEnumerator onRiverEvent()
        {
            var boardConfig = JsonObjects.getBoardConfig();
            var participantsInfo = JsonObjects.getParticipantsInfo();

            StaticVariables.boardConfig = boardConfig;
            StaticVariables.participantsInfoMessage = participantsInfo;
            
            
            SceneManager.LoadScene("Scenes/Game");
            yield return null;
            var gameManager = GameObject.Find("_manager").GetComponent<GameManager>();
            
            gameManager.onRiverEvent(JsonObjects.getRiverEvent());
            
        }
        
        [UnityTest]
        public IEnumerator onShotEvent()
        {
            var boardConfig = JsonObjects.getBoardConfig();
            var participantsInfo = JsonObjects.getParticipantsInfo();

            StaticVariables.boardConfig = boardConfig;
            StaticVariables.participantsInfoMessage = participantsInfo;
            
            
            SceneManager.LoadScene("Scenes/Game");
            yield return null;
            var gameManager = GameObject.Find("_manager").GetComponent<GameManager>();
            
            gameManager.onGameState(JsonObjects.getGameState());
            yield return new WaitForSeconds(5);
            gameManager.onShotEvent(JsonObjects.getShotEvent());
            yield return new WaitForSeconds(5);

            
        }
        
        [UnityTest]
        public IEnumerator onCardEvent()
        {
            var boardConfig = JsonObjects.getBoardConfig();
            var participantsInfo = JsonObjects.getParticipantsInfo();

            StaticVariables.boardConfig = boardConfig;
            StaticVariables.participantsInfoMessage = participantsInfo;
            
            
            SceneManager.LoadScene("Scenes/Game");
            yield return null;
            var gameManager = GameObject.Find("_manager").GetComponent<GameManager>();
            
            gameManager.onCardEvent(JsonObjects.getCardEvent());
            
        }
    }
}

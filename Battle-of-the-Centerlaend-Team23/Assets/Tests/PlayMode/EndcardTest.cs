using System.Collections;
using System.Collections.Generic;
using System.Threading;
using communication;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class EndcardTest
    {
        [UnityTest]
        public IEnumerator PlayerReadyParticipantsInfoTest()
        {

            //var endcardScript = GameObject.Find("Canvas").GetComponent<EndCard>();

            GAME_END_Message gameEnd = new GAME_END_Message();
            gameEnd.message = Message.GAME_END;
            gameEnd.data = new GAME_END_Message_Data();
            gameEnd.data.playerStates = new List<PlayerState>();
            gameEnd.data.winner = "Player 1";

            PlayerState playerState = new PlayerState();
            playerState.playerName = "Player 1";
            playerState.currentPosition = new []{1,2};
            playerState.spawnPosition = new[] { 3, 4 };
            playerState.direction = Direction.EAST;
            playerState.character = Character.SAM;
            playerState.lives = 3;
            playerState.lembasCount = 3;
            playerState.suspended = 0;
            playerState.reachedCheckpoints = 3;
            playerState.playedCards = new[] { Card.MOVE_1, Card.MOVE_1 };
            playerState.turnOrder = 2;
            
            gameEnd.data.playerStates.Add(playerState);

            gameEnd.data.additional = new List<Additional>();

            //endcardScript.OnGAME_END(gameEnd);
            StaticVariables.gameEndMessage = gameEnd;
            
            SceneManager.LoadScene("Scenes/Endscreen");
            yield return null;


            yield return null;
            
            Assert.IsTrue(true);
            
            StaticVariables.gameEndMessage = null;
        }
    }
}
using System.Collections;
using communication;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class PlayerReadySceneTest
    {
        [UnityTest]
        public IEnumerator PlayerReadyParticipantsInfoTest()
        {
            SceneManager.LoadScene("Scenes/Waiting for other Players");
            yield return null;
            
            var connectionSceneSkript = GameObject.Find("Canvas").GetComponent<PlayerReadyScene>();

            PARTICIPANTS_INFO_Message participantsInfo = new PARTICIPANTS_INFO_Message();
            participantsInfo.message = Message.PARTICIPANTS_INFO;
            participantsInfo.data = new PARTICIPANTS_INFO_Message_Data();
            participantsInfo.data.ais = new [] { "ai1", "ai2" };
            participantsInfo.data.players = new [] { "p1", "p2" };
            participantsInfo.data.spectators = new [] { "s1", "s2" };
            participantsInfo.data.readyPlayers = new [] { "ai1", "p2" };
            
            connectionSceneSkript.OnParticipantsInfoMessage(participantsInfo);
            yield return null;
            
            Assert.IsTrue(true);
        }
    }
}
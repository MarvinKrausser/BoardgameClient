using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using communication;
using managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests.PlayMode
{
    public class ShortGame
    {
        private MessageManager mManager;
        
        public bool OpenServer()
        {
            TestingCommunication2 testServer = new TestingCommunication2();
            testServer.TestingCommunicationServer();
            return true;
        }
        
        public bool CloseServer()
        {
            mManager.closeWebsocket();
            
            return true;
        }
        
        [UnityTest]
        public IEnumerator ConnectionSceneTest()
        {
            OpenServer();
            
            var boardConfig = JsonObjects.getBoardConfig();
            var participantsInfo = JsonObjects.getParticipantsInfo();

            StaticVariables.boardConfig = boardConfig;
            StaticVariables.participantsInfoMessage = participantsInfo;
            
            MessageManager.gameStateSemaphore = new SemaphoreSlim(5);
            MessageManager.participantsInfoSemaphore = new SemaphoreSlim(5);
            MessageManager.reconnectSemephore = new SemaphoreSlim(5);
            MessageManager.cardOfferSemephore = new SemaphoreSlim(5);
            
            SceneManager.LoadScene("Scenes/Connect Server");
            yield return null;
            var connectionSceneSkript = GameObject.Find("Canvas").GetComponent<ConnectionScene>();
            mManager = GameObject.Find("MessageManager").GetComponent<MessageManager>();
            
            //checking Update Method
            ConnectionScene.errorMessage0 = "HELLO";
            ConnectionScene.errorCode2 = true;
            ConnectionScene.errorCode3 = true;
            ConnectionScene.errorCode4 = true;
            ConnectionScene.errorCode5 = true;
            yield return null;
            ConnectionScene.errorMessage0 = null;
            ConnectionScene.errorCode2 = false;
            ConnectionScene.errorCode3 = false;
            ConnectionScene.errorCode4 = false;
            ConnectionScene.errorCode5 = false;
            yield return null;
            //checking Update Method end
            
            connectionSceneSkript.ConnectButton();
            connectionSceneSkript.defaultValues = true;
            connectionSceneSkript.ConnectButton();
            yield return null;

            Thread.Sleep(1000);
            GameObject sw = new GameObject("SceneSwitcher");
            sw.AddComponent<SceneSwitcher>();
            
            SceneManager.LoadScene("Scenes/Game");
            yield return null;
            
            Thread.Sleep(200);
            bool isWebsocketOpen1 = mManager.isWebsocketOpen();
            Debug.Log("Sending Message 1");
            mManager._writeMessage.WriteMessageHELLO_SERVER("name", Role.PLAYER);
            

            Thread.Sleep(200);
            isWebsocketOpen1 = isWebsocketOpen1 && mManager.isWebsocketOpen();
            Debug.Log("Sending Message 2");
            mManager._writeMessage.WriteMessagePLAYER_READY(true);

            Thread.Sleep(200);
            isWebsocketOpen1 = isWebsocketOpen1 && mManager.isWebsocketOpen();
            Debug.Log("Sending Message 3");
            mManager._writeMessage.WriteMessageCHARACTER_CHOICE(Character.GANDALF);


            Thread.Sleep(200);
            isWebsocketOpen1 = isWebsocketOpen1 && mManager.isWebsocketOpen();
            Debug.Log("Sending Message 4");
            mManager._writeMessage.WriteMessageCARD_CHOICE(new Card[] { Card.MOVE_1 ,Card.MOVE_2,Card.MOVE_3,Card.LEMBAS, Card.MOVE_2});

            Thread.Sleep(200);
            mManager._writeMessage.WriteMessagePAUSE_REQUEST(true);
            
            Thread.Sleep(200);
            mManager._writeMessage.WriteMessagePAUSE_REQUEST(false);
            
            Thread.Sleep(200);

            
            CloseServer();

            bool isWebsocketOpen2 = mManager.isWebsocketOpen();
            
            //mManager._cardOfferMessage = null;
            MessageManager.gameStateSemaphore = new SemaphoreSlim(0);
            MessageManager.participantsInfoSemaphore = new SemaphoreSlim(0);
            MessageManager.reconnectSemephore = new SemaphoreSlim(0);
            MessageManager.cardOfferSemephore = new SemaphoreSlim(0);
            
            Assert.AreEqual(isWebsocketOpen1, !isWebsocketOpen2);

            SceneManager.LoadScene("Scenes/Game"); //resetting the scene, because there are some unimportand nullreferences. We don't need to test this, because we don't have here those scripts loaded. We just want to test here the communication!
            yield return null;
        }
    }
}
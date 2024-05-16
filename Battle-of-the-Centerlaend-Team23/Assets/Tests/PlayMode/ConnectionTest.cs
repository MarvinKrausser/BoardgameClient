using System;
using System.Collections;
using System.Threading;
using communication;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    public class ConnectionTest
    {
        private Communication _communication;
        private ReadMessage _readMessage;
        private WriteMessage _writeMessage;
        
        
        public bool OpenServer()
        {
            TestingCommunication testServer = new TestingCommunication();
            testServer.TestingCommunicationServer();
            return true;
        }
        
        public bool CloseServer()
        {
            
            TestingCommunication.StopWebSocketServer(); //kam bei mir zu einem error den ich nicht fixen konnte...irgend was mit der referenz
            _communication.CloseWebSocketConnection();
            
            return true;
        }
        
        [UnityTest]
        public IEnumerator Connection()
        {
            OpenServer();
            
            _communication = new Communication("127.0.0.1", 3018);
            _readMessage = _communication.rMessage;
            _writeMessage = new WriteMessage(_communication);
            Debug.Log("Opend Connection");
            
            Thread.Sleep(100);
            
            bool isWebsocketOpen1 = _communication.isWebsocketOpen();
            
            _writeMessage.WriteMessageHELLO_SERVER("Spieler1", Role.PLAYER); 
            _writeMessage.WriteMessagePLAYER_READY(true);
            _writeMessage.WriteMessageCHARACTER_CHOICE(Character.MERRY);
            _writeMessage.WriteMessageRECONNECT("Spieler1", Guid.NewGuid());
            _writeMessage.WriteMessageGOODBYE_SERVER();
            _writeMessage.WriteMessageCARD_CHOICE(new Card [] {Card.MOVE_1, Card.U_TURN, Card.MOVE_2, Card.MOVE_3, Card.AGAIN});
            _writeMessage.WriteMessagePAUSE_REQUEST(true);
            Debug.Log("Wrote all Messages");
            
            isWebsocketOpen1 =  isWebsocketOpen1 && _communication.isWebsocketOpen();
            
            Thread.Sleep(5000);
            
            Debug.Log("Received All Messages");
            
            CloseServer();
            bool isWebsocketOpen2 = _communication.isWebsocketOpen();
            
            Assert.AreEqual(isWebsocketOpen1, !isWebsocketOpen2);
            yield return null;
        }
        
        
        
    }
}
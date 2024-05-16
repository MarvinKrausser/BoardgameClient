using System;
using System.Threading;
using Newtonsoft.Json;
using UnityEngine;

namespace communication
{
    /// <summary>
    /// The other halve of the communication to the server. Responsible for sending the correct messages to the server.
    /// </summary>
    public class WriteMessage 
    {
        private Communication _communication;

        public WriteMessage(Communication com)
        {
            _communication = com;
        }
        private void SendMessage<T>(T message)
        {
            //Debug.Log(JsonConvert.SerializeObject(message));
            _communication.SendMessage(JsonConvert.SerializeObject(message));
        }
        /// <summary>
        /// Sends the Hello_Server message.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="role"></param>
        public void WriteMessageHELLO_SERVER(string name, Role role)
        {
            HELLO_SERVER_Message message = new HELLO_SERVER_Message();
            message.message = Message.HELLO_SERVER;
            message.data = new HELLO_SERVER_Message_Data();
            message.data.name = name;
            message.data.role = role;
            
            SendMessage(message);
        }
        
        /// <summary>
        /// Sends the Ready_Player message
        /// </summary>
        /// <param name="ready"></param>
        public void WriteMessagePLAYER_READY(bool ready)
        {
            PLAYER_READY_Message message = new PLAYER_READY_Message();
            message.message = Message.PLAYER_READY;
            message.data = new PLAYER_READY_Message_Data();
            message.data.ready = ready; 
            
            SendMessage(message);
        }

        /// <summary>
        /// Sends the Character_Choice message
        /// </summary>
        /// <param name="characterChoice"></param>
        public void WriteMessageCHARACTER_CHOICE(Character characterChoice)
        {
            CHARACTER_CHOICE_Message message = new CHARACTER_CHOICE_Message();
            message.message = Message.CHARACTER_CHOICE;
            message.data = new CHARACTER_CHOICE_Message_Data();
            message.data.characterChoice = characterChoice; 

            SendMessage(message);
        }
        
        /// <summary>
        /// And so on
        /// </summary>
        /// <param name="name"></param>
        /// <param name="reconnectToken"></param>
        public void WriteMessageRECONNECT(string name, Guid reconnectToken) //for example GUID: "Guid.NewGuid()"
        {
            RECONNECT_Message message = new RECONNECT_Message();
            message.message = Message.RECONNECT;
            message.data = new RECONNECT_Message_Data();
            message.data.name = name;
            message.data.reconnectToken = reconnectToken;
            
            SendMessage(message);

        }

        /// <summary>
        /// and so on
        /// </summary>
        public void WriteMessageGOODBYE_SERVER()
        {
            GOODBYE_SERVER_Message message = new GOODBYE_SERVER_Message();
            message.message = Message.GOODBYE_SERVER;
            message.data = new GOODBYE_SERVER_Message_Data();
            
            SendMessage(message);
        }

        /// <summary>
        /// It is always the same
        /// </summary>
        /// <param name="cards"></param>
        public void WriteMessageCARD_CHOICE(Card[] cards)
        {
            CARD_CHOICE_Message message = new CARD_CHOICE_Message();
            message.message = Message.CARD_CHOICE;
            message.data = new CARD_CHOICE_Message_Data();
            message.data.cards = cards;
            //message.data.cards = new Card[] { Card.MOVE_1, Card.MOVE_1, Card.MOVE_1, Card.MOVE_1, Card.MOVE_1 };

            SendMessage(message);
        }

        /// <summary>
        /// No one will ever read this, but if you do: This sends the Pause_Request to the server.
        /// </summary>
        /// <param name="pause"></param>
        public void WriteMessagePAUSE_REQUEST(bool pause)
        {
            PAUSE_REQUEST_Message message = new PAUSE_REQUEST_Message();
            message.message = Message.PAUSE_REQUEST;
            message.data = new PAUSE_REQUEST_Message_Data();
            message.data.pause = pause;
            
            SendMessage(message);
        }
        
    }

}
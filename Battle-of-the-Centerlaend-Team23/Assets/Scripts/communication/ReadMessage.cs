using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Unity.VisualScripting;
using UnityEngine;

namespace communication
{
    /// <summary>
    /// This is where the magic happens. The messages are read, parsed and validated, then the respective events and their methods are called.
    /// </summary>
    public class ReadMessage
    {
        public delegate void OnHELLO_CLIENTMessage(HELLO_CLIENT_Message message);

        public delegate void OnPARTICIPANTS_INFOMessage(PARTICIPANTS_INFO_Message message);

        public delegate void OnGAME_STARTMessage(GAME_START_Message message);

        public delegate void OnCHARACTER_OFFERMessage(CHARACTER_OFFER_Message message);

        public delegate void OnGAME_STATEMessage(GAME_STATE_Message message);

        public delegate void OnERRORMessage(ERROR_Message message);

        public delegate void OnINVALID_MESSAGEMessage(INVALID_MESSAGE_Message message);

        public delegate void OnCARD_OFFERMessage(CARD_OFFER_Message message);

        public delegate void OnROUND_STARTMessage(ROUND_START_Message message);

        public delegate void OnCARD_EVENTMessage(CARD_EVENT_Message message);

        public delegate void OnSHOT_EVENTMessage(SHOT_EVENT_Message message);

        public delegate void OnRIVER_EVENTMessage(RIVER_EVENT_Message message);

        public delegate void OnGAME_ENDMessage(GAME_END_Message message);

        public delegate void OnPAUSEDMessage(PAUSED_Message message);

        public delegate void OnEAGLE_EVENTMessage(EAGLE_EVENT_Message message);

        public event OnHELLO_CLIENTMessage OnHELLO_CLIENT;
        public event OnPARTICIPANTS_INFOMessage OnPARTICIPANTS_INFO;
        public event OnGAME_STARTMessage OnGAME_START;
        public event OnCHARACTER_OFFERMessage OnCHARACTER_OFFER;
        public event OnGAME_STATEMessage OnGAME_STATE;
        public event OnERRORMessage OnERROR;
        public event OnINVALID_MESSAGEMessage OnINVALID_MESSAGE;
        public event OnCARD_OFFERMessage OnCARD_OFFER;
        public event OnROUND_STARTMessage OnROUND_START;
        public event OnCARD_EVENTMessage OnCARD_EVENT;
        public event OnSHOT_EVENTMessage OnSHOT_EVENT;
        public event OnRIVER_EVENTMessage OnRIVER_EVENT;
        public event OnGAME_ENDMessage OnGAME_END;
        public event OnPAUSEDMessage OnPAUSED;
        public event OnEAGLE_EVENTMessage OnEAGLE_EVENT;
        
        

        private static Dictionary<Message, string> _schemas = new ();
        
        
        ///Constructor is just used to initialize the Dictionary _schemas#
        public ReadMessage()
        {
            _schemas = new ();
            string relativePath;
            
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                relativePath = "Resources/Data/StreamingAssets/Schemas/";
            }
            else 
            {
                relativePath = "StreamingAssets/Schemas/";
            }
            
            string path = Path.Combine(Application.dataPath, relativePath);

            //string path = Directory.GetCurrentDirectory() + "\\..\\..\\..\\Schemas\\"; //Local Project
            
            foreach (Message m in Enum.GetValues(typeof(Message)))
            {
                using (StreamReader sr =
                       new StreamReader(path + m.ToString().ToLower().Replace("_", String.Empty) + ".schema.json"))
                {
                    string schemaJason = sr.ReadToEnd();
                    _schemas.Add(m, schemaJason);
                }
            }
        }
        
        /// <summary>
        /// checks the input string if it is valid and if it's a valid message, abd then it calls the correct event
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void ReadMessages(string message)
        {
            //get the type of the message
            Message? messsageType = GetMessageType(message);
            //is Message valid? if not, then ignore the message
            if (!IsValid(message, messsageType)) return; 
            
            switch (messsageType)
            {
                case Message.HELLO_CLIENT:
                    ReadMessageHELLO_CLIENT(message);
                    break;
                case Message.PARTICIPANTS_INFO:
                    ReadMessagePARTICIPANTS_INFO(message);
                    break;
                case Message.GAME_START:
                    ReadMessageGAME_START(message);
                    break;
                case Message.CHARACTER_OFFER:
                    ReadMessageCHARACTER_OFFER(message);
                    break;
                case Message.GAME_STATE:
                    ReadMessageGAME_STATE(message);
                    break;
                case Message.ERROR:
                    ReadMessageERROR(message);
                    break;
                case Message.INVALID_MESSAGE:
                    ReadMessageINVALID_MESSAGE(message);
                    break;
                case Message.CARD_OFFER:
                    ReadMessageCARD_OFFER(message);
                    break;
                case Message.ROUND_START:
                    ReadMessageROUND_START(message);
                    break;
                case Message.CARD_EVENT:
                    ReadMessageCARD_EVENT(message);
                    break;
                case Message.SHOT_EVENT:
                    ReadMessageSHOT_EVENT(message);
                    break;
                case Message.RIVER_EVENT:
                    ReadMessageRIVER_EVENT(message);
                    break;
                case Message.GAME_END:
                    ReadMessageGAME_END(message);
                    break;
                case Message.PAUSED:
                    ReadMessagePAUSED(message);
                    break;
                case Message.EAGLE_EVENT:
                    ReadMessageEAGLE_EVENT(message);
                    break;
                default:
                    throw new InvalidOperationException("The send message is invalid");
            }
        }
        
        
        /// <summary>
        /// checks if the json string from a given type is valid
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsValid(string json, Message? type)
        {
            if (type is null) return false;
            JObject jObject = JObject.Parse(json);
            string schemaJson = _schemas[(Message)type];
                
            //Parses the schema - returns whether successful or not
            JSchema schema = JSchema.Parse(schemaJson);
            if (jObject.IsValid(schema))
            {
                Debug.Log("Message is valid");
                return true;
            }
            Debug.Log("Message is not valid: "+jObject);
            return false;
        }

        /// <summary>
        /// this method gives the Message Type (for example HELLO_CLIENT Message)
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private Message? GetMessageType(string json)
        {
            //Parse Object
            JObject jObject;
            try
            {
                jObject = JObject.Parse(json);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
            if (jObject.First is null) return null;
            string? type = jObject.First.ToObject<string>();
            if (type is null) return null;
            if (Enum.IsDefined(typeof(Message), type)) return Enum.Parse<Message>(type);

            return null;
        }

        /// <summary>
        /// Reads the HELLO_CLIENT Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageHELLO_CLIENT(string message)
        {
            HELLO_CLIENT_Message jsonFile = JsonConvert.DeserializeObject<HELLO_CLIENT_Message>(message);

            OnHELLO_CLIENT?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the PARTICIPANTS_INFO Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessagePARTICIPANTS_INFO(string message)
        {
            PARTICIPANTS_INFO_Message jsonFile = JsonConvert.DeserializeObject<PARTICIPANTS_INFO_Message>(message);

            OnPARTICIPANTS_INFO?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the GAME_START Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageGAME_START(string message)
        {
            GAME_START_Message jsonFile = JsonConvert.DeserializeObject<GAME_START_Message>(message);

            OnGAME_START?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the CHARACTER_OFFER Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageCHARACTER_OFFER(string message)
        {
            CHARACTER_OFFER_Message jsonFile = JsonConvert.DeserializeObject<CHARACTER_OFFER_Message>(message);

            OnCHARACTER_OFFER?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the GAME_START Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageGAME_STATE(string message)
        {
            GAME_STATE_Message jsonFile = JsonConvert.DeserializeObject<GAME_STATE_Message>(message);

            OnGAME_STATE?.Invoke(jsonFile);
        }

        /// <summary>
        /// Reads the ERROR Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageERROR(string message)
        {
            ERROR_Message jsonFile = JsonConvert.DeserializeObject<ERROR_Message>(message);

            OnERROR?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the INVALID_MESSAGE Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageINVALID_MESSAGE(string message)
        {
            INVALID_MESSAGE_Message jsonFile = JsonConvert.DeserializeObject<INVALID_MESSAGE_Message>(message);

            OnINVALID_MESSAGE?.Invoke(jsonFile);
        }

        /// <summary>
        /// Reads the CARD_OFFER Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageCARD_OFFER(string message)
        {
            CARD_OFFER_Message jsonFile = JsonConvert.DeserializeObject<CARD_OFFER_Message>(message);

            OnCARD_OFFER?.Invoke(jsonFile);
        }

        /// <summary>
        /// Reads the ROUND_START Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageROUND_START(string message)
        {
            ROUND_START_Message jsonFile = JsonConvert.DeserializeObject<ROUND_START_Message>(message);

            OnROUND_START?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the CARD_EVENT Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageCARD_EVENT(string message)
        {
            CARD_EVENT_Message jsonFile = JsonConvert.DeserializeObject<CARD_EVENT_Message>(message);

            OnCARD_EVENT?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the SHOT_EVENT Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageSHOT_EVENT(string message)
        {
            SHOT_EVENT_Message jsonFile = JsonConvert.DeserializeObject<SHOT_EVENT_Message>(message);

            OnSHOT_EVENT?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the RIVER_EVENT Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageRIVER_EVENT(string message)
        {
            RIVER_EVENT_Message jsonFile = JsonConvert.DeserializeObject<RIVER_EVENT_Message>(message);

            OnRIVER_EVENT?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the GAME_END Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageGAME_END(string message)
        {
            GAME_END_Message jsonFile = JsonConvert.DeserializeObject<GAME_END_Message>(message);

            OnGAME_END?.Invoke(jsonFile);
        }
        
        /// <summary>
        /// Reads the PAUSED Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessagePAUSED(string message)
        {
            PAUSED_Message jsonFile = JsonConvert.DeserializeObject<PAUSED_Message>(message);

            OnPAUSED?.Invoke(jsonFile);
        }

        /// <summary>
        /// Reads the EAGLE_EVENT Message in and calls the event
        /// </summary>
        /// <param name="message"></param>
        public void ReadMessageEAGLE_EVENT(string message)
        {
            EAGLE_EVENT_Message jsonFile = JsonConvert.DeserializeObject<EAGLE_EVENT_Message>(message);

            OnEAGLE_EVENT?.Invoke(jsonFile);
        }
        
        
        public static void reset_schemas()
        {
            _schemas = new Dictionary<Message, string>();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using communication;
using managers;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;

using WebSocketSharp;
using ErrorEventArgs = WebSocketSharp.ErrorEventArgs;

namespace communication
{
    /// <summary>
    /// This class is responsible for the low level communication between the client and the server.
    /// It opens, handles, closes the websocket and subscribes to the events.
    /// </summary>
    public class Communication
{

    private static WebSocket _webSocket;
    public static Queue<string> _messageQueue = new Queue<string>();
    public ReadMessage rMessage;

    private SemaphoreSlim _messageSemaphore;

    /// <summary>
    /// Instantiates the MessageHandler which is responsible for opening and closing the websocket, as well as handling the messages.
    /// </summary>
    /// <param name="ipAdress"></param>
    /// <param name="port"></param>
    public Communication(string ipAdress, int port)
    {
        MessageHandler(ipAdress, port); 
    }

    /// <summary>
    /// Websocket-sharp event OnMessage: is called, when the client receives a message from the server.
    /// It puts the data into a queue and calls in a new Thread the "ReceivedMessage()" Method, which handels the messages.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Ws_OnMessage(object sender, MessageEventArgs e)
    {
        _messageQueue.Enqueue(e.Data);
        
        new Thread(() => ReceivedMessage()).Start();
    }
    
    /// <summary>
    /// Websocket-sharp event OnClose: is called, when the websocket has an error.
    /// It switches the scene to the connection lost scene, if the client has not received the game end message.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Ws_OnClose(object sender, CloseEventArgs args)
    {
        try
        {
            if (MessageManager.connectionAttempts > 0)
            {
                MessageManager.connectionAttempts--;
                //Debug.Log("decrement. New Value: " + MessageManager.connectionAttempts);
            }

            Thread.Sleep(2000);
            if (!(MessageManager.activeScene is null || MessageManager.activeScene.Equals("Connect Server")))
            {
                if (MessageManager.connectionAttempts == 0 && (MessageManager.activeScene.Equals("Game") ||
                                                               MessageManager.activeScene.Equals("Characterselection") ||
                                                               MessageManager.activeScene.Equals(
                                                                   "Waiting for other Players")))
                {
                    Debug.Log("OnClose: end or lost connection to Server");
                    ConnectionLost.errorText = "Verbindung verloren";
                    MessageManager.switchToConnectionLost = true;
                }
            }
            Debug.Log("OnClose: WebSocket closed 1");
        }
        catch (Exception e)
        {
            Debug.Log(e.Data);
            Debug.Log("OnClose: WebSocket closed 2");
        }
    }
    
    /// <summary>
    /// Websocket-sharp event OnError: is called, when the websocket has an error.
    /// It switches the scene to the connection lost scene, if its not already done
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Ws_OnError(object sender, ErrorEventArgs args)
    {
        Thread.Sleep(2000);
        if (MessageManager.activeScene.Equals("Game") ||
            MessageManager.activeScene.Equals("Characterselection") ||
            MessageManager.activeScene.Equals("Waiting for other Players"))
        {
            Debug.Log("OnError: end or lost connection to Server");
            MessageManager.switchToConnectionLost = true;
        }

        Debug.Log("ERROR: WebSocket closed");
    }
    
    /// <summary>
    /// Websocket-sharp event OnOpen: is called, when the websocket is initialized
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Ws_OnOpen(object sender, EventArgs args)
    {
        Debug.Log("Connected to server");
    }

    private void WebSocketConnect()
    {
        _webSocket.Connect();
    }

    /// <summary>
    /// Creates the websocket (with Websocket-sharp) to a given ip and portnumber
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="portNumber"></param>
    private void MessageHandler(string ip, int portNumber)
    {
        _messageSemaphore = new SemaphoreSlim(1);
        
        string ipAdress = ip; 
        int port = portNumber; //3018;
        
        try
        {
            _webSocket = new WebSocket("ws://"+ ipAdress +":"+ port +"/"); 
            Debug.Log("created Websocket");
            
            _webSocket.OnMessage += Ws_OnMessage;
            _webSocket.OnClose += Ws_OnClose;
            _webSocket.OnError += Ws_OnError;
            _webSocket.OnOpen += Ws_OnOpen;
            
            rMessage = new ReadMessage();
            
            new Thread(() => WebSocketConnect()).Start();

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }


    /// <summary>
    /// The Client reads the messages from the Server. 
    /// </summary>
    public void ReceivedMessage()
    {
        try 
        {
            if (_messageQueue != null && _messageQueue.Count != 0)
            { 
                _messageSemaphore.Wait(); //waits until the semaphore allows to go in the critical section

                try
                {
                    var message = _messageQueue.Dequeue();
                    Debug.Log("Message Received: " + message);
                    
                    rMessage.ReadMessages(message);
                }
                finally
                {
                    _messageSemaphore.Release(); //Release the semaphore
                }

            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    /// <summary>
    /// sending Message to Server. Important! The message must be converted to a string before we can call this method
    /// </summary>
    /// <param name="message"></param>
    public void SendMessage(string message)
    {
        try
        {
            _webSocket.Send(message);
        }
        catch (Exception e)
        {
            Debug.Log("There is no connection to a server");
        }
    }

    /// <summary>
    /// Returns if the websocket is still connected
    /// </summary>
    /// <returns></returns>
    public bool isWebsocketOpen()
    {
        return _webSocket.IsAlive;
    }
    
    /// <summary>
    /// Closes the websocket connection
    /// </summary>
    public void CloseWebSocketConnection()
    {
        _webSocket.Close();
    }

}
}



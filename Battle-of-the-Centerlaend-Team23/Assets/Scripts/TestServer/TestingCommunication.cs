using System;
using System.IO;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

//using NUnit.Framework;

public class Echo : WebSocketBehavior
{
    protected override void OnOpen()
    {
        Thread.Sleep(2000);
        for (int i = 0; i <= 14; i++)
        {
            StreamReader sReader = new StreamReader(TestingCommunication.GetPath(i)); //which file should be read in
            bool isReading = true;
            String fullMessage = "";
            while (isReading) //&& client.Connected)
            {
                String line = sReader.ReadLine();
                if (line == null || line.Equals("") ) 
                {
                    isReading = false;
                }
                else
                {
                    fullMessage += line + "\r\n";
                }
            }
            //Debug.Log("Server: " + fullMessage);
            
            Send(fullMessage);
            Debug.Log("Server sendet data");
            Thread.Sleep(100); //TODO: If you want to test, that the Server receives the message correctly, than this line should be commented out
        }
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Server received message: "+ e.Data);
    }
}

public class TestingCommunication
{
    private static WebSocketServer _wssv;
    
    public void TestingCommunicationServer()
    {
        _wssv = new WebSocketServer("ws://127.0.0.1:3018");

        _wssv.AddWebSocketService<Echo>("/");

        _wssv.Start();
        Debug.Log("WS server startet on ws://127.0.0.1:3018");
    }

    public static void StopWebSocketServer()
    {
        _wssv.Stop();
        Debug.Log("Stop WebSocketServer");
    }
    
    public static string GetPath(int messageNumber)
    {
        switch (messageNumber)
        {
            case 0: return "Assets/Scripts/TestServer/HELLO_CLIENT_Example01.json";
            case 1: return "Assets/Scripts/TestServer/PARTICIPANTS_INFO_Example01.json"; 
            case 2: return "Assets/Scripts/TestServer/GAME_START_Example01.json";
            case 3: return "Assets/Scripts/TestServer/CHARACTER_OFFER_Example01.json";
            case 4: return "Assets/Scripts/TestServer/GAME_STATE_Example01.json";
            case 5: return "Assets/Scripts/TestServer/ERROR_Example01.json";
            case 6: return "Assets/Scripts/TestServer/INVALID_MESSAGE_Example01.json";
            case 7: return "Assets/Scripts/TestServer/CARD_OFFER_Example01.json"; 
            case 8: return "Assets/Scripts/TestServer/ROUND_START_Example01.json";
            case 9: return "Assets/Scripts/TestServer/CARD_EVENT_Example01.json";
            case 10: return "Assets/Scripts/TestServer/SHOT_EVENT_Example01.json";
            case 11: return "Assets/Scripts/TestServer/RIVER_EVENT_Example01.json";
            case 12: return "Assets/Scripts/TestServer/GAME_END_Example01.json";
            case 13: return "Assets/Scripts/TestServer/PAUSED_Example01.json";
            case 14: return "Assets/Scripts/TestServer/EAGLE_EVENT_Example01.json";
        }

        return "";
    }
}

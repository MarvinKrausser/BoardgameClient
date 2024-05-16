using System;
using System.IO;
using System.Threading;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

//using NUnit.Framework;

public class Echo2 : WebSocketBehavior
{
    private SemaphoreSlim testingSemaphore = new SemaphoreSlim(0);
    
    protected override void OnOpen()
    {
        new Thread(sendMessagesToClient).Start();
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Server received message: " + e.Data);
        testingSemaphore.Release();
    }

    public void sendMessagesToClient()
    {
        testingSemaphore.Wait();

        string fullMessage = TestingCommunication2.ReadExampleMessage(0);
        Send(fullMessage);
        Debug.Log("Server sent Hello_client");
        
        fullMessage = TestingCommunication2.ReadExampleMessage(1);
        Send(fullMessage);
        Debug.Log("Server sent Participants Info 1");
        
        testingSemaphore.Wait();
        
        fullMessage = TestingCommunication2.ReadExampleMessage(1);
        Send(fullMessage);
        Debug.Log("Server sent Participants Info 2");
        
        fullMessage = TestingCommunication2.ReadExampleMessage(2);
        Send(fullMessage);
        Debug.Log("Server sent Game Start");
        
        fullMessage = TestingCommunication2.ReadExampleMessage(3);
        Send(fullMessage);
        Debug.Log("Server sent Character Offer");
        
        testingSemaphore.Wait();
        
        fullMessage = TestingCommunication2.ReadExampleMessage(4);
        Send(fullMessage);
        Debug.Log("Server sent Game State");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(5);
        Send(fullMessage);
        Debug.Log("Server sent Error Message");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(7);
        Send(fullMessage);
        Debug.Log("Server sent Card Offer Message");
        
        testingSemaphore.Wait();
        
        fullMessage = TestingCommunication2.ReadExampleMessage(8);
        Send(fullMessage);
        Debug.Log("Server sent Round Start Message");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(9);
        Send(fullMessage);
        Debug.Log("Server sent Card Event Message");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(10);
        Send(fullMessage);
        Debug.Log("Server sent Shot Event Message");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(11);
        Send(fullMessage);
        Debug.Log("Server sent River Event Message");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(14);
        Send(fullMessage);
        Debug.Log("Server sent Eagle Event Message");
        
        Thread.Sleep(100);
        
        testingSemaphore.Wait();
        
        fullMessage = TestingCommunication2.ReadExampleMessage(13);
        Send(fullMessage);
        Debug.Log("Server sent Paused true Message");
        
        testingSemaphore.Wait();
        
        fullMessage = TestingCommunication2.ReadExampleMessage(15);
        Send(fullMessage);
        Debug.Log("Server sent Paused false Message");
        
        Thread.Sleep(100);
        
        fullMessage = TestingCommunication2.ReadExampleMessage(12);
        Send(fullMessage);
        Debug.Log("Server sent Game End Message");
        
        Thread.Sleep(100);

        TestingCommunication2.StopWebSocketServer();

    }
}

public class TestingCommunication2
{
    private static WebSocketServer _wssv;
    
    public void TestingCommunicationServer()
    {
        _wssv = new WebSocketServer("ws://127.0.0.1:3018");

        _wssv.AddWebSocketService<Echo2>("/");

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
            case 5: return "Assets/Scripts/TestServer/ERROR_Example02.json";
            case 6: return "Assets/Scripts/TestServer/INVALID_MESSAGE_Example01.json";
            case 7: return "Assets/Scripts/TestServer/CARD_OFFER_Example01.json"; 
            case 8: return "Assets/Scripts/TestServer/ROUND_START_Example01.json";
            case 9: return "Assets/Scripts/TestServer/CARD_EVENT_Example01.json";
            case 10: return "Assets/Scripts/TestServer/SHOT_EVENT_Example01.json";
            case 11: return "Assets/Scripts/TestServer/RIVER_EVENT_Example01.json";
            case 12: return "Assets/Scripts/TestServer/GAME_END_Example01.json";
            case 13: return "Assets/Scripts/TestServer/PAUSED_Example01.json";
            case 14: return "Assets/Scripts/TestServer/EAGLE_EVENT_Example01.json";
            case 15: return "Assets/Scripts/TestServer/PAUSED_Example02.json";
        }

        return "";
    }

    public static string ReadExampleMessage(int i)
    {
        
        StreamReader sReader = new StreamReader(GetPath(i)); //which file should be read in
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

        return fullMessage;
    }
}

using System;
using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;

public class wstest : MonoBehaviour {

  bool prefetch;
  public string host = "192.168.1.223";
  public int port = 1200;
  string address;
  string error = "";
  string message = "";
  WebSocket webSocket;

  // Use this for initialization
  void Start () {
    address  = "ws://"+host+":"+port+"/";

    Debug.Log("starting " + address);
    prefetch = Security.PrefetchSocketPolicy(host, 843, 3000);

    webSocket = new WebSocket(address);
    webSocket.OnOpen += OnOpen;
    webSocket.OnMessage += OnMessageReceived;
    webSocket.OnClose += OnClose;
    webSocket.OnError += OnError;
    webSocket.Connect();
  }

  void OnGUI() {
    GUI.Label(new Rect(0,0,Screen.width,Screen.height),"prefetch "+prefetch+"\nwebsocket " + webSocket +"\nerror "+error+"\nmessage " +message);
  }
  
  // Update is called once per frame
  void Update () {
  }

  void OnOpen(object sender, System.EventArgs e) {
    message = "open";
    Debug.Log("open");
    webSocket.Send("hello");
  }

  void OnMessageReceived(object sender, MessageEventArgs e)
  {
    message = e.Data.ToString();
    Debug.Log("websocket message " + message);
  }

  void OnClose(object sender, CloseEventArgs e)
  {
    Debug.Log("websocket closed  " + UnityEngine.StackTraceUtility.ExtractStackTrace());
  }

  void OnError(object sender, ErrorEventArgs e)
  {
    error = e.Message;
  }
}

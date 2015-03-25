using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using WebSocketSharp;


public class TestWS : MonoBehaviour {
	private WebSocket ws;
	Queue q = new Queue();

	void Start () {
//		bool wsSecurityPolicy = Security.PrefetchSocketPolicy("192.168.1.59", 843); // WebSockets
		Security.PrefetchSocketPolicy("192.168.1.59", 843);
		ws = new WebSocket ("ws://edison.local:1200");
		ws.OnMessage += OnMessage;
		ws.OnError += OnError;
		ws.OnClose += OnClose;

		// Debug
		ws.Log.Level = LogLevel.TRACE;
		ws.Log.File = "unity-websocket.log";

		// Connect
		ws.Connect ();
		ws.Send ("Unity connected");	
	}
	
	void OnMessage (object sender, MessageEventArgs e) {
		Debug.Log ("<color=blue>Message</color> " + e.Data);
		q.Enqueue(e.Data.ToString());
	}
	
	void OnError (object sender, ErrorEventArgs e) {
		Debug.Log ("<color=red>ERROR</color> WebSocketClient: " + e.Message);
	}
	
	void OnClose (object sender, CloseEventArgs e) {
		Debug.Log ("<color=yellow>Closed</color> WebSocketClient: " + e.Reason);
	}
	
	void Update () {
	}
}

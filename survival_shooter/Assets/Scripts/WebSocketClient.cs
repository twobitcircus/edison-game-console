using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
//using ScoreManager;


public class WebSocketClient : MonoBehaviour {
    private WebSocket ws;
    Queue q = new Queue();

    GameObject player;
    PlayerShooting gunScript;
    PlayerHealth playerHealth;
    Dictionary<string,bool> input_state = new Dictionary<string,bool>();
    Dictionary<string,float> axis = new Dictionary<string,float>();

    public string host = "192.168.1.223";
    public int port = 1200;
    string address;

    string message;

    void Start ()
    {
      address  = "ws://"+host+":"+port+"/";
      Security.PrefetchSocketPolicy(host, 843, 3000);

      ws = new WebSocket (address); // Original port 8080

      ws.OnOpen += OnOpen;        
      ws.OnMessage += OnMessage;
      ws.OnError += OnError;
      ws.OnClose += OnClose;

      //ws.Log.Level = LogLevel.TRACE;
      //ws.Log.File = "unity-websocket.log";
          
      ws.Connect ();      

      //player = GameObject.FindGameObjectWithTag ("Player");
      //gunScript = player.GetComponentInChildren <PlayerShooting> ();
      //playerHealth = player.GetComponentInChildren <PlayerHealth> ();

    }

    void OnOpen (object sender, System.EventArgs e) {
      Debug.Log ("Open Connection WebSocketClient");
    }
    
    void OnMessage (object sender, MessageEventArgs e) {
      Debug.Log ("External controller" + e.Data);
      q.Enqueue(e.Data.ToString());
    }

    void OnError (object sender, ErrorEventArgs e) {
      Debug.Log ("ERROR WebSocketClient: " + e.Message);
    }
        
    void OnClose (object sender, CloseEventArgs e) {
      Debug.Log ("Closed WebSocketClient: " + e.Reason);
    }

    void OnGUI() {
      GUI.Label(new Rect(0,0,Screen.width,Screen.height),"message " +message);
    }


    void Update () 
    {
      while (q.Count > 0) {
        string msg = q.Dequeue() as string;

        message = msg;
      
        switch (msg) {
          case "A": 
          case "B": input_state["Fire1"] = true;  gunScript.Shoot();  break;
          case "a": 
          case "b": input_state["Fire1"] = false; gunScript.Shoot();  break;

          case "U": axis["vertical"] = 1;    break;
          case "D": axis["vertical"] = -1;   break;
          case "L": axis["horizontal"] = 1;  break;
          case "R": axis["horizontal"] = -1; break;

          case "u": 
          case "d": axis["vertical"] = 0;   break;
          case "l": 
          case "r": axis["horizontal"] = 0; break;
        }
      }

      // Dead? (Game Over)
      //if (playerHealth.currentHealth < 1) {
      //  ws.Send("gameover");
      //} else {
      //  // Send score update
      //  //ws.Send("" + ScoreManager.score);
      //}
    }

    public bool GetButton(string name) {
      return input_state[name];
    }

    public float GetAxis(string _axis){
      if (!axis.ContainsKey(_axis)) {
        return 0;
      } else {
        if (_axis == "horizontal") {
          if (axis[_axis] == 1) {
            return -1;
          } else if (axis[_axis] == -1) {
            return 1;
          } else {
            return 0;
          }
        } else {
          return axis[_axis];
        }
      }
    }
    
}


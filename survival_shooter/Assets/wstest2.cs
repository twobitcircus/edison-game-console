
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.WebSocket;

public class wstest2 : MonoBehaviour {

  bool prefetch;
  public string host = "192.168.1.223";
  public int port = 1200;
  string address;
  string error = "";
  string message = "";

  Queue q = new Queue();
  GameObject player;
  PlayerShooting gunScript;
  PlayerHealth playerHealth;
  Dictionary<string,bool> input_state = new Dictionary<string,bool>();
  Dictionary<string,float> axis = new Dictionary<string,float>();

  WebSocket webSocket;

  // Use this for initialization
  void Start () {
    address  = "ws://"+host+":"+port+"/";

    Debug.Log("starting " + address);
    prefetch = Security.PrefetchSocketPolicy(host, 843, 3000);

    webSocket = new WebSocket(new Uri(address));

    webSocket.OnOpen += OnOpen;
    webSocket.OnMessage += OnMessageReceived;
    webSocket.OnClosed += OnClosed;
    webSocket.OnError += OnError;

    webSocket.Open();
  }

  void OnGUI() {
    GUI.Label(new Rect(0,0,Screen.width,Screen.height),"message " +message);
  }
  
  // Update is called once per frame
  void Update () {
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
  }

  void OnOpen(WebSocket ws) {
    message = "open";
    Debug.Log("open");
    webSocket.Send("hello");
  }

  void OnMessageReceived(WebSocket ws, string msg) {
    message = msg;
    q.Enqueue(msg.ToString());
    Debug.Log("websocket message " + message);
  }

  void OnClosed(WebSocket ws, UInt16 code, string message) {
    Debug.Log("websocket closed message " + message);// " + UnityEngine.StackTraceUtility.ExtractStackTrace());
  }

  void OnError(WebSocket ws, Exception ex)
  {
    Debug.Log("error");
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

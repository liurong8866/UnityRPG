
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetDebug : MonoBehaviour {
	public bool debug;
	public static NetDebug netDebug;

    public bool IsWuDi = false;

	List<string> consoleDebug = new List<string>();
	public void AddConsole(string msg) {
		consoleDebug.Add (msg);
		if (consoleDebug.Count > 20) {
			consoleDebug.RemoveAt(0);
		}
	}

	void Awake() {
		netDebug = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	void OnGUI() {
		if (debug) {
			GUILayout.BeginVertical();
			GUILayout.TextField(string.Join("\n", KBEngine.Bundle.sendMsg.ToArray()));

			GUILayout.EndVertical();

			GUI.TextField(new Rect(Screen.width*3.0f/4, 0, Screen.width/4, Screen.height/2),string.Join("\n", KBEngine.Bundle.recvMsg.ToArray()));
			GUI.TextField(new Rect(0, Screen.height*3/4.0f, Screen.width/4, Screen.height/4), string.Join("\n", consoleDebug.ToArray()));
		}

	}
	// Update is called once per frame
	void Update () {

	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public interface IMainLoop {
    void queueInLoop(Action cb);
}

public class MainThreadLoop : MonoBehaviour, IMainLoop {
    List<System.Action> pendingCallbacks = new List<Action>();

	// Use this for initialization
	void Start () {
	
	}
    public void queueInLoop(System.Action cb){
        lock(pendingCallbacks) {
            pendingCallbacks.Add(cb);
        }
    }
	
	// Update is called once per frame
	void Update () {
        lock(pendingCallbacks) {
            foreach(var p in pendingCallbacks) {
                try {
                    p();
                }catch(Exception e) {
                    Debug.LogError("CallBacks: "+e.ToString());
                }
            }
            pendingCallbacks.Clear();
        }
	}
}

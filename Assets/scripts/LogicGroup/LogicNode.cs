using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(CommandHandler))]
public class LogicNode : MonoBehaviour {
    public static Dictionary<int, GameObject> allNodes = new Dictionary<int, GameObject>();
    static int nodeId = 0;
    protected int myId;

    public LogicGroup logicGroup = null;
    void Awake(){
        myId = nodeId++;
        allNodes[myId] = gameObject;

        var ch = GetComponent<CommandHandler>();
        ch.AddHandler("set_group", SetGroup);
    }

    protected virtual void Start(){
    }

    void SetGroup(CommandHandler.Command cmd) {
        logicGroup = LogicGroup.allGroups[Convert.ToInt32(cmd.cmd[1])];
    }

    void OnDestroy(){
        allNodes.Remove(myId);
    }

}

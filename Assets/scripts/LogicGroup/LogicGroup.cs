using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CommandHandler))]
public class LogicGroup : MonoBehaviour {
    public static Dictionary<int, LogicGroup> allGroups = new Dictionary<int, LogicGroup>();
    static int groupId = 0;

    int myId;
    public List<LogicLink> links = new List<LogicLink>();
    void Awake() {
        myId = groupId++;
        allGroups[myId] = this;
    }
	
    void Start () {
        var ch = GetComponent<CommandHandler>();
        ch.AddHandler("input_event", OnEvent);
	    
        
        foreach(var l in links) {
            if(l.source != null) {
                l.source.GetComponent<CommandHandler>().AddCommand(string.Format("set_group {0}", myId));
            }
        }
    }

	void OnEvent(CommandHandler.Command cmd) {
        var evt = cmd.cmd[1];
        foreach(var l in links) {
            if(l.output == evt && l.source == LogicNode.allNodes[System.Convert.ToInt32(cmd.cmd[2])]){
                l.Handle();
            }
        }
    }

    void OnDestroy() {
        allGroups.Remove(myId);
    }

	
}

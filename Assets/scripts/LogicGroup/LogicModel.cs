using UnityEngine;
using System.Collections;

public class LogicModel : LogicNode {
    public string ani;

	// Use this for initialization
	protected override void Start () {
        GetComponent<Animation>()[ani].speed = 1;
        GetComponent<Animation>()[ani].wrapMode = WrapMode.Loop;
        GetComponent<Animation>().CrossFade(ani);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void HIT(){
        logicGroup.GetComponent<CommandHandler>().AddCommand(string.Format("input_event actived {0}", myId));
    }
}

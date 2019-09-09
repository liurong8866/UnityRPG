using UnityEngine;
using System.Collections;


public class LogicParticle : LogicNode {

	// Use this for initialization
	protected override void Start () {
        base.Start();

        var ch = GetComponent<CommandHandler>();

        ch.AddHandler("start_particle", OnStart);
	}

	void OnStart(CommandHandler.Command cmd) {
        Debug.Log(gameObject+" show particle ");
        //gameObject.SetActive(true);
        GetComponent<XffectComponent>().Active();
        GetComponent<XffectComponent>().enabled = false;
        GetComponent<XffectComponent>().Reset();
        StartCoroutine(ShowParticle());
    }
    IEnumerator ShowParticle(){
        yield return null;
        GetComponent<XffectComponent>().enabled = true;
    }
}

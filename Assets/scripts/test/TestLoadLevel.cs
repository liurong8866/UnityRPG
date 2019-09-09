using UnityEngine;
using System.Collections;
using MyLib;

public class TestLoadLevel : MonoBehaviour {
    void Awake(){
        if (SaveGame.saveGame == null) {
            var g = new GameObject();
            var s = g.AddComponent<SaveGame>();
            s.InitData();
        }
    }
	// Use this for initialization
	void Start () {
        StartCoroutine(InitStreamLoadLevel());
	}
    IEnumerator InitStreamLoadLevel(){

        yield return new WaitForSeconds(3);
        SaveGame.saveGame.TestSetRole();
        yield return null;

        //StartCoroutine(WorldManager.worldManager.TestInitScene());

        yield return null;
    }

	
	// Update is called once per frame
	void Update () {
	
	}
}

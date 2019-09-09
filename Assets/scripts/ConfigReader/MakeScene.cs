using UnityEngine;
using System.Collections;

public class MakeScene : MonoBehaviour {
    public static MakeScene makeScene;
    public string modelStr;
    public string dataConfigStr;
    public string lightStr;
    void Awake() {
        makeScene = this;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

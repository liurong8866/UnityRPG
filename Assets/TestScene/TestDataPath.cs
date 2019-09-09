using UnityEngine;
using System.Collections;

public class TestDataPath : MonoBehaviour {
    #if UNITY_EDITOR
    [ButtonCallFunc()]
    public bool Path;
    public void PathMethod() {
        Debug.LogError(Application.dataPath);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    #endif
}

using UnityEngine;
using System.Collections;

public class PlayerStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach(Transform t in transform) {
            t.gameObject.SetActive(false);
        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

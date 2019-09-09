
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

public class SetCamPos : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameObject g = GameObject.FindGameObjectWithTag("LightMapCamera");
		Vector3 pos = g.transform.position;
		Vector3 diff = pos - Camera.main.transform.position;

		GetComponent<Renderer>().material.SetVector("_CamPos", new Vector4(diff.x, diff.y, diff.z, 0));
	}
}

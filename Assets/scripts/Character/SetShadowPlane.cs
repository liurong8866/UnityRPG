
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


public class SetShadowPlane : MonoBehaviour {
	public GameObject plane;

	// Update is called once per frame
	void Update () {
		for(int i = 0; i < GetComponent<Renderer>().materials.Length; i++)
		{
			GetComponent<Renderer>().materials[i].SetMatrix("_World2Receiver", plane.GetComponent<Renderer>().worldToLocalMatrix);
		}
	}
}

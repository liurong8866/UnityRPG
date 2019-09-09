
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

[ExecuteInEditMode]
public class PlaneCollision : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnPlaneCollision(Xft.CollisionParam param) {
		//GameObject g = param.CollideObject;
		//Vector3 pos = param.CollidePos;
		Xft.EffectNode ef = param.effectNode;
		ef.SetBounce();
		//Debug.Log("collision is "+g+" "+pos);
	}

}

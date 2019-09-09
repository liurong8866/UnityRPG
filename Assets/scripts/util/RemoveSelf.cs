
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

public class RemoveSelf : MonoBehaviour {
    public GameObject connectDestory;
	void Start () {
	}
	void OnDisable() {
		GameObject.Destroy(gameObject);
        if(connectDestory != null) {
            GameObject.Destroy(connectDestory);
        }
	}
}

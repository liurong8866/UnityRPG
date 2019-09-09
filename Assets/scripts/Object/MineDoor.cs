
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

namespace MyLib {
	public class MineDoor : MonoBehaviour {
		bool opend = false;
		Collider col;
		void Awake() {
			col = Util.FindChildRecursive (transform, "collision").GetComponent<Collider> ();
		}
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			if (!opend) {
				RaycastHit hit;

				var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Debug.DrawRay(ray.origin, ray.origin+ray.direction*100);
				if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
					
					Debug.Log("hit who? " +hit.collider);
					if (hit.collider == col) {
						opend = true;
						GetComponent<Animation>().CrossFade("closed");
						col.enabled = false;
					}
				}
			}
		}
	}

}
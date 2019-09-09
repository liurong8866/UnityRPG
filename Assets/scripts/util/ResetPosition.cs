
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
	public class ResetPosition : MonoBehaviour {
		[ButtonCallFunc()]
		public bool ResetPos;
		public void ResetPosMethod() {
			transform.localPosition = Vector3.zero;
			foreach (Transform t in transform) {
				t.localPosition = Vector3.zero;

			}
		}
	
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}

}
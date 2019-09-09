
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib {
	public class ActivityController : MonoBehaviour {
		public static ActivityController activityController;
		void Awake() {
			activityController = this;
			DontDestroyOnLoad (gameObject);
		}

	}
}

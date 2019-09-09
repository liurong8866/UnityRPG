
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib {
	public class SoulTower : MonoBehaviour {
		public static SoulTower soulTower;
		void Awake() {
			soulTower = this;
			DontDestroyOnLoad (gameObject);
		}
	}

}
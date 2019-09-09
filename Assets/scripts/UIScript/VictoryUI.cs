
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

namespace MyLib {
	public class VictoryUI : IUserInterface {
		public GameObject contGame;
		public bool con = false;

		void Awake() {
			SetCallback ("button", OnContinue);
			con = false;
		}

		void OnContinue(GameObject g) {
			con = true;
		}


	}

}
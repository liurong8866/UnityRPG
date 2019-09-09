using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class MonsterCharacter : AICharacter
	{
		public override void SetRun() {
			Log.AI ("Monster run "+GetAttr().gameObject.name);
			var runName = "run";
			if (!CheckAni (runName)) {
				runName = "walk";
			}
			SetAni (runName, 2, WrapMode.Loop);
		}
		public override void SetIdle() {
			SetAni ("idle", 1, WrapMode.Loop);
		}
	}
}

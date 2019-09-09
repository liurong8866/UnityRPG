using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class HumanDead : DeadState
	{
		string dieAni;
		public override void EnterState ()
		{
			base.EnterState ();
			dieAni = "death";
			SetAni (dieAni, 1, WrapMode.Once);
            GetAttr().IsDead = true;
		}
		public override IEnumerator RunLogic ()
		{
			yield return GetAttr ().StartCoroutine (Util.WaitForAnimation(GetAttr().GetComponent<Animation>()));
			var evt = new MyEvent (MyEvent.EventType.PlayerDead);
			evt.localID = aiCharacter.GetLocalId ();
			MyEventSystem.myEventSystem.PushEvent (evt);
		}
	}
}

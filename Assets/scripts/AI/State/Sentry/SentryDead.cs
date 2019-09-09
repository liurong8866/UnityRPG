using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class SentryDead : DeadState
	{
		public override void EnterState()
		{
			base.EnterState();
			SetAni("die", 1, WrapMode.Once);
		}
		
		public override IEnumerator RunLogic()
		{
			while(GetAttr().GetComponent<Animation>().isPlaying) {
				yield return null;
			}
			ObjectManager.objectManager.DestroyByLocalId (GetAttr().GetComponent<KBEngine.KBNetworkView>().GetLocalId());
		}
	}
}

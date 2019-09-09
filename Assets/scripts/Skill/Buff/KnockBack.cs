using UnityEngine;
using System.Collections;

namespace MyLib
{
	//0.8s 击退怪物 buff
	public class KnockBack : IEffect
	{
		public override void Init (Affix af, GameObject o)
		{
			base.Init (af, o);
			type = Affix.EffectType.KnockBack;
		}

		public override void OnActive ()
		{
			Log.AI ("KnockBack Buff Active");
            obj.GetComponent<MyAnimationEvent> ().KnockBackWho (attackerPos);
		}

	}

}
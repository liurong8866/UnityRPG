using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class SummonDuration : IEffect
	{
		public override void Init (Affix af, GameObject o)
		{
			base.Init (af, o);
			type = Affix.EffectType.SummonDuration;
		}

		public override void OnDie ()
		{
			base.OnDie ();
			obj.GetComponent<MyAnimationEvent>().timeToDead = true;
		}
	}
}

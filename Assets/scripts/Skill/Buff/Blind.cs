using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Blind : IEffect
    {
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            type = Affix.EffectType.Blind;
        }

        public override void OnActive ()
        {
            Log.AI ("Stunned Buff Active");
            GraphInit.Instance.SetBlind(true);
        }

        public override void OnDie()
        {
            base.OnDie();
            GraphInit.Instance.SetBlind(false);
        }
	
    }
}
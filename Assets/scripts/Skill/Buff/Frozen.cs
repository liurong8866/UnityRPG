using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Frozen : IEffect 
    {
        public override void Init (Affix af, GameObject o)
        {
            base.Init (af, o);
            type = Affix.EffectType.Frozen;
        }
        public override float GetSpeedCoff()
        {
            return 0.5f;
        }
    }

}
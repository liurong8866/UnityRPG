﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 获得一次性暴击BUff  Buff监听Hit事件
    /// </summary>
    public class OneTimeCriticalHit : IEffect 
    {
        public override void Init (Affix af, GameObject o)
        {
            base.Init (af, o);
            type = Affix.EffectType.OneTimeCriticalHit;
            
        }
        public override void OnActive()
        {
            base.OnActive();
            obj.GetComponent<MyAnimationEvent>().AddCallBackLocalEvent(MyEvent.EventType.HitTarget, HitTarget);
        }

        void HitTarget(MyEvent evt) {
            IsDie = true;
        }

        public override int GetCriticalRate()
        { 
            return 100;
        }

        public override void OnDie()
        {
            base.OnDie();
            obj.GetComponent<MyAnimationEvent>().DropCallBackLocalEvent(MyEvent.EventType.HitTarget, HitTarget);
        }

    }

}
﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 攻击速度 200%
    /// 暴击率+30% 
    /// </summary>
    public class ShiXue : IEffect
    {
        public override void Init (Affix af, GameObject o)
        {
            base.Init (af, o);
            type = Affix.EffectType.ShiXue;
            BackgroundSound.Instance.PlayEffect("skills/shockhit");
        }
        public override void OnActive()
        {
            base.OnActive();
            GetBuffCom().StartCoroutine(ChangeSize());
        }
        IEnumerator ChangeSize() {
            float t = 0;
            while(t < 1) {
                obj.transform.localScale = Mathf.Lerp(1, 2, t)*Vector3.one ;
                t += Time.deltaTime;
                yield return null;
            }
        }
        public override float GetSpeedCoff()
        {
            return 3;
        }

        public override int GetCriticalRate()
        { 
            return 50;
        }
    }

}
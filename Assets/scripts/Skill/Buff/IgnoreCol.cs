﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 只忽略碰撞但是伤害计算还是要算的 
    /// </summary>
    public class IgnoreCol : IEffect
    {
        public override void Init(Affix af, GameObject o)
        {
            base.Init(af, o);
            obj.layer = (int) GameLayer.IgnoreCollision2;

        }

        public override void OnDie()
        {
            base.OnDie();
            obj.layer = (int) GameLayer.Npc;
        }
    }

}
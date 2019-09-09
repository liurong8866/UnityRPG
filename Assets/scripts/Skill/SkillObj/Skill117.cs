﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 忍者闪烁条件判定
    /// 和目标敌人距离相距超过10
    /// </summary>
    public class Skill117 : SkillObj
    {
        public override bool CheckCondition(GameObject owner)
        {
            var target = owner.GetComponent<CommonAI>().targetPlayer;
            if(target != null) 
            {
                var diff = Util.XZSqrMagnitude(owner.transform.position, target.transform.position);
                Log.Sys("Skill117 diff "+diff);
                if(diff > 100) {
                    return true;
                }
            }
            return false;
        }
    }

}
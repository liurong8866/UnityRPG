﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 检测技能使用的前置条件是否满足 如果满足则开始释放 
    /// </summary>
    public class SkillObj : MonoBehaviour
    {
        public virtual bool CheckCondition(GameObject owner){
            return true;
        }
    }

}
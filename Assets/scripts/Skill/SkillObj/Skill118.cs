using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class Skill118 : SkillObj 
    {
        /// <summary>
        /// 没有反弹Buff 才释放这个技能 
        /// </summary>
        /// <returns><c>true</c>, if condition was checked, <c>false</c> otherwise.</returns>
        /// <param name="owner">Owner.</param>
        public override bool CheckCondition(GameObject owner)
        {
            var buf = owner.GetComponent<BuffComponent>();
            return !buf.CheckHasSuchBuff(Affix.EffectType.FanTan);
        }

    }

}
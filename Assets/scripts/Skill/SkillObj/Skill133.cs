using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///嗜血狼技能
    /// 因为技能状态伴随 整个 怪物生命周期 因此 不用删除注册事件 
    /// </summary>
    public class Skill133 : SkillObj
    {
        float lastTime = 0;
        bool useYet = false;
        void Awake() {
            GetComponent<MyAnimationEvent>().AddCallBackLocalEvent(MyEvent.EventType.HitTarget, HitTarget);
        }
        int hitNum = 0;
        void HitTarget(MyEvent evt) {
            hitNum++;
        }

        void OnDestroy() {
            GetComponent<MyAnimationEvent>().DropCallBackLocalEvent(MyEvent.EventType.HitTarget, HitTarget);
        }

        /// <summary>
        /// 远离攻击目标 
        /// </summary>
        /// <returns><c>true</c>, if condition was checked, <c>false</c> otherwise.</returns>
        /// <param name="owner">Owner.</param>
        public override bool CheckCondition(GameObject owner)
        {
            if (Time.time - lastTime > 3)
            {
                if(hitNum >= 1 && !useYet){
                    useYet = true;
                    return true;
                }
            }
            return false;
        }

       
    }
}

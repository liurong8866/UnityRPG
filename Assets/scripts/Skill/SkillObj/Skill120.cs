using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///7s一次 每次持续5s 
    /// </summary>
    public class Skill120 : SkillObj 
    {
        float lastTime = 0;
        public override bool CheckCondition(GameObject owner)
        {
            var diff = Time.time-lastTime;
            if(diff >= 8) {
                lastTime = Time.time;
                return true;
            }
            return false;
        }
    }
}

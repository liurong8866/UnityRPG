using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// hp低的时候释放 
    /// </summary>
    public class Skill126 : SkillObj 
    {
        float lastTime = 0;
        float sucTime = 0;
        public override bool CheckCondition(GameObject owner)
        {
            if(Time.time-lastTime > 5  && Time.time-sucTime > 30) {
                lastTime = Time.time;
                var npc = owner.GetComponent<NpcAttribute>();
                if(npc.HP < npc.HP_Max*0.8f) {
                    sucTime = Time.time;
                    return true;
                }
            }
            return false;
        }

    }

}
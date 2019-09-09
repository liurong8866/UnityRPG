using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    ///组合技能触发武器Buff 
    /// 统计连续技能的释放
    /// 最多统计5个技能
    /// </summary>
    public class SkillCombineBuff : MonoBehaviour
    {
        List<int> skills = new List<int>();

        int localId;
        void Awake()
        {
            
        }

        // Use this for initialization
        void Start()
        {
            localId = GetComponent<NpcAttribute>().GetLocalId();
            MyEventSystem.myEventSystem.RegisterLocalEvent(localId, MyEvent.EventType.HitTarget, OnEvent);
        }
        void OnEvent(MyEvent evt) {
            if(evt.type == MyEvent.EventType.HitTarget) {
                skills.Add(evt.skill.Id);
                MatchTest();
                RemoveMore();//移除超时的还多余的
            }
        }
        void MatchTest() {
        }
        void RemoveMore() {
        }
        void OnDestroy() {
            MyEventSystem.myEventSystem.DropLocalListener(localId, MyEvent.EventType.HitTarget, OnEvent);
        }
	
    }

}
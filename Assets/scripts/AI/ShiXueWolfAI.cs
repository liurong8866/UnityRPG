using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ShiXueWolfAI : AIBase
    {
        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            var heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);
            
            ai = new MonsterCharacter();
            ai.attribute = attribute;
            ai.AddState(new MonsterIdle());
            ai.AddState(new MonsterCombat());
            ai.AddState(new MonsterDead());
            ai.AddState(new MonsterKnockBack());
            ai.AddState(new MonsterSkill());

            //var ani = GetComponent<MyAnimationEvent>();
            //ani.AddCallBackLocalEvent(MyEvent.EventType.Hit, HitOther);
        }
        /*
        void HitOther()
        {
        
        }
        */

        //出生释放133 嗜血技能
        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            //SkillLogic.CreateSkillStateMachine(gameObject, Util.GetSkillData(133, 1), transform.position, null);
            gameObject.AddComponent<MonsterCheckUseSkill>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (attribute.IsDead)
            {
                Util.ClearMaterial(gameObject);
            }
        }
    }

}
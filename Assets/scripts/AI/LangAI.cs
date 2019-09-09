using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class LangAI : AIBase
    {

        MyAnimationEvent myAnimationEvent;
        float heading;
        Vector3 targetRotation;

        void Awake()
        {
            attribute = GetComponent<NpcAttribute>();
            myAnimationEvent = GetComponent<MyAnimationEvent>();
            heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);
            
            ai = new MonsterCharacter();
            ai.attribute = attribute;
            ai.AddState(new MonsterIdle());
            ai.AddState(new MonsterCombat());
            ai.AddState(new MonsterDead());
            ai.AddState(new MonsterFlee());
            ai.AddState(new MonsterKnockBack());
            ai.AddState(new MonsterSkill());

            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>() {
                MyEvent.EventType.WolfCall,
            };
            RegEvent();
        }

        protected override void OnEvent(MyEvent evt)
        {
            if (evt.type == MyEvent.EventType.WolfCall)
            {
                if (evt.localID != attribute.GetLocalId())
                {
                    GetComponent<CommonAI>().targetPlayer = evt.target;
                    var diff = Util.XZSqrMagnitude(evt.target.transform.position, transform.position);
                    if (diff > 25)
                    {
                        var aniEvent = myAnimationEvent;
                        var skillData = Util.GetSkillData(117, 1);
                        aniEvent.InsertMsg(new MyAnimationEvent.Message(MyAnimationEvent.MsgType.IDLE));
                        aniEvent.OnSkill(skillData);
                    }
                }
            }
        }

        void Start()
        {
            ai.ChangeState(AIStateEnum.IDLE);
            
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
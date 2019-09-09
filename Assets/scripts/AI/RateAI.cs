using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class RateAI : AIBase 
    {
        void Awake() {
            attribute = GetComponent<NpcAttribute>();
            var heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);
            
            ai = new MonsterCharacter ();
            ai.attribute = attribute;
            ai.AddState (new MonsterIdle  ());
            ai.AddState(new MonsterCombat ());
            ai.AddState (new ReliveDead  ());
            ai.AddState (new MonsterKnockBack ());
            ai.AddState(new MonsterSkill());

            ai.canRelive = true;

            this.regEvt = new System.Collections.Generic.List<MyEvent.EventType>() {
                MyEvent.EventType.HuiShuLeaderDead,
            };
            RegEvent();
        }

        protected override void OnEvent(MyEvent evt)
        { 
            if (evt.type == MyEvent.EventType.HuiShuLeaderDead)
            {
                ai.canRelive = false;
            }
        }
        
        void Start() {
            ai.ChangeState (AIStateEnum.IDLE);
        }
        
        protected override void OnDestroy() {
            base.OnDestroy();
            if (attribute.IsDead) {
                Util.ClearMaterial (gameObject);
            }
        } 

    }

}
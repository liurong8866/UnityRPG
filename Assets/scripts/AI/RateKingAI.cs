using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class RateKingAI : AIBase 
    {
        void Awake() {
            attribute = GetComponent<NpcAttribute>();
            var heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);

            ai = new MonsterCharacter ();
            ai.attribute = attribute;
            ai.AddState (new MonsterIdle ());
            ai.AddState (new MonsterCombat ());
            ai.AddState (new MonsterDead ());
            ai.AddState (new MonsterFlee ());
            ai.AddState (new MonsterKnockBack ());
            ai.AddState(new MonsterSkill());
            MyEventSystem.myEventSystem.RegisterLocalEvent(attribute.GetLocalId(), MyEvent.EventType.MeDead, OnDead);
        }

        void OnDead(MyEvent evt) {
            if(evt.type == MyEvent.EventType.MeDead) {
                MyEventSystem.myEventSystem.PushEvent(MyEvent.EventType.HuiShuLeaderDead);
            }
        }

        void Start() {
            ai.ChangeState (AIStateEnum.IDLE);
        }

        protected override void OnDestroy() {
            MyEventSystem.myEventSystem.DropLocalListener(attribute.GetLocalId(), MyEvent.EventType.MeDead, OnDead);
            base.OnDestroy();
            if (attribute.IsDead) {
                Util.ClearMaterial (gameObject);
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SnakeAI : AIBase
    {

        void Awake() {
            attribute = GetComponent<NpcAttribute>();
            var heading = Random.Range(0, 360);
            transform.eulerAngles = new Vector3(0, heading, 0);

            ai = new MonsterCharacter ();
            ai.attribute = attribute;
            ai.AddState (new SnakeIdle  ());
            ai.AddState(new SnakeMove());
            ai.AddState (new MonsterDead ());
            ai.AddState (new MonsterKnockBack ());
        }

        void Start() {
            ai.ChangeState (AIStateEnum.IDLE);
            SkillLogic.CreateSkillStateMachine(gameObject, Util.GetSkillData(128, 1), transform.position, null);
        }
        
        protected override void OnDestroy() {
            base.OnDestroy();
            if (attribute.IsDead) {
                Util.ClearMaterial (gameObject);
            }
        }

    }

}
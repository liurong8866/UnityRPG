using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class ThiefAI : AIBase 
    {

        void Awake() {
            attribute = GetComponent<NpcAttribute>();

            ai = new MonsterCharacter ();
            ai.attribute = attribute;
            ai.AddState (new MonsterIdle ());
            ai.AddState (new MonsterCombat ());
            ai.AddState (new MonsterDead ());
            ai.AddState (new MonsterFlee ());
            ai.AddState (new MonsterKnockBack ());
            ai.AddState(new MonsterSkill());
        }

        void Start()
        {
            ai.ChangeState (AIStateEnum.IDLE);
            StartCoroutine(CheckRush());
        }


        /// <summary>
        ///检查是否释放冲击技能 
        /// </summary>
        /// <returns>The rush.</returns>
        IEnumerator CheckRush(){
            var commonAI = GetComponent<CommonAI>();

            while(true) {
                var target = commonAI.targetPlayer;
                var state = ai.state.type;
                if(target != null && !target.GetComponent<NpcAttribute>().IsDead && state == AIStateEnum.COMBAT) 
                {
                    var msg = new MyAnimationEvent.Message();
                    msg.type = MyAnimationEvent.MsgType.DoSkill;
                    var sd = Util.GetSkillData(142, 1);
                    msg.skillData = sd;
                    GetComponent<MyAnimationEvent>().InsertMsg(msg);
                }
                yield return new WaitForSeconds(5);
            }
        }


        protected override void OnDestroy() {
            base.OnDestroy();
            if (attribute.IsDead) {
                Util.ClearMaterial (gameObject);
            }
        }

    }

}
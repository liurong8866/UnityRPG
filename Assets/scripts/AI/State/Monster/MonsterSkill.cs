using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class MonsterSkill : SkillState 
    {
        SkillStateMachine skillStateMachine;
        SkillFullInfo activeSkill;
        float FastRotateSpeed = 10;
        float walkSpeed = 3.0f;

        public override void EnterState ()
        {
            base.EnterState ();
            Log.AI ("Enter Skill State ");
            //启动技能状态机 启动动画
            activeSkill = GetAttr ().GetComponent<SkillInfoComponent> ().GetActiveSkill ();
            skillStateMachine = SkillLogic.CreateSkillStateMachine(GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position);
            //GetAttr ().GetComponent<AnimationController> ().SetAnimationSampleRate (100);
        }
        public override void ExitState ()
        {
            //GetAttr ().GetComponent<AnimationController> ().SetAnimationSampleRate (30);
            base.ExitState ();
        }

        //向当前所面朝方向进行攻击
        public override IEnumerator RunLogic ()
        {
            Log.AI ("Check Animation "+GetAttr().GetComponent<Animation>().IsPlaying(activeSkill.skillData.AnimationName));
            float passTime = 0;
            //var realAttackTime = GetAttr ().ObjUnitData.AttackAniSpeed;
            var realAttackTime = activeSkill.skillData.AttackAniTime/GetAttr().GetSpeedCoff();
            var animation = GetAttr ().GetComponent<Animation>();
            var attackAniName = activeSkill.skillData.AnimationName;
            var rate = GetAttr().GetComponent<Animation>()[attackAniName].length/realAttackTime;
            Log.AI("AttackAniSpeed "+rate+" realAttackTime "+realAttackTime);
            PlayAni (activeSkill.skillData.AnimationName, rate, WrapMode.Once);
            
            var playerMove = GetAttr ().GetComponent<MoveController> ();
            var physics = playerMove.GetComponent<PhysicComponent>();

            var targetPlayer = GetAttr().GetComponent<CommonAI>().targetPlayer;

            while(!quit) {
                if(CheckEvent()) {
                    break;
                }

                Vector3 dir = targetPlayer.transform.position - GetAttr ().transform.position;
                dir.y = 0;
                var newDir = Vector3.Slerp(GetAttr().transform.forward, dir, Time.deltaTime * FastRotateSpeed );
                physics.TurnTo(newDir);

                if(passTime >= animation[attackAniName].length*0.8f/animation[attackAniName].speed) {
                    break;
                }
                passTime += Time.deltaTime;
                yield return null;
            }
            
            MyEventSystem.myEventSystem.PushLocalEvent(GetAttr().GetLocalId(), MyEvent.EventType.AnimationOver);
            Log.AI ("Stop SkillState ");
            skillStateMachine.Stop();
            aiCharacter.ChangeState (AIStateEnum.IDLE);
        }

    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    public class HumanCombat : CombatState
    {
        #region ConstVar

        float WindowTime = 0.5f;
        float rotateSpeed = 10;
        float walkSpeed = 1.5f;

        #endregion


        bool pressAttack = false;
        float attackPressTime;
        int attackId = 0;
        string attackAniName = null;
        SkillStateMachine skillStateMachine;
        SkillFullInfo activeSkill;

        //注册监听玩家的OnHit事件处理
        public override void EnterState()
        {
            Log.AI("Enter HumanCombat state");
            base.EnterState();

            pressAttack = false;
            attackPressTime = Time.time;
            attackId = 0;
            SetAttrState(CharacterState.Attacking);

            activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();

        }

        public override void ExitState()
        {
            Log.AI("Push HideWeaponTrail");
            MyEventSystem.myEventSystem.PushLocalEvent(GetAttr().GetLocalId(), MyEvent.EventType.HideWeaponTrail);
            base.ExitState();
        }

        string GetAttackAniName()
        {
            var name = string.Format("rslash_{0}", attackId + 1);
            attackId++;
            attackId %= 4;
            return name;
        }

        public override bool CheckNextState(AIStateEnum next)
        {
            if (next == AIStateEnum.COMBAT)
            {
                Log.AI("Combat receive attack again");
                attackPressTime = Time.time;
                pressAttack = true;
                return false;
            }
            if (next == AIStateEnum.CAST_SKILL)
            {
                return true;
            }
            return base.CheckNextState(next);
        }

        /*
		 * 伤害计算过程
		1：伤害对象判定  客户端做
		2：伤害数值确定   服务端 或者客户端 
		3：伤害效果施展 例如击退  服务端 或者 客户端
		*/

        //TODO:增加摇杆控制攻击方向功能 这样人物会根据摇杆方向来确定攻击目标
        IEnumerator WaitForAttackAnimation(Animation animation)
        {
            var playerMove = GetAttr().GetComponent<MoveController>();
            var camRight = playerMove.camRight;
            var camForward = playerMove.camForward;
            var vcontroller = playerMove.vcontroller;
            var physics = playerMove.GetComponent<PhysicComponent>();

            var rd = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffect("onehandswinglarge" + rd);
            skillStateMachine = SkillLogic.CreateSkillStateMachine(GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position);
            Log.AI("Wait For Combat Animation");
            //GameObject enemy = NearestEnemy ();
            float passTime = 0;
            //var transform = GetAttr ().transform;
            bool hitYet = false;
            do
            {
                if (!hitYet && GetEvent().onHit)
                {
                    hitYet = true;
                }
                if (CheckEvent())
                {
                    break;
                }

                float v = 0;
                float h = 0;
                if (vcontroller != null)
                {
                    h = vcontroller.inputVector.x;//CameraRight 
                    v = vcontroller.inputVector.y;//CameraForward
                }

                if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
                {
                    Vector3 moveDirection = playerMove.transform.forward;
                    Vector3 targetDirection = h * camRight + v * camForward;
                    if (targetDirection != Vector3.zero)
                    {
                        moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Time.deltaTime, 0);			
                    }
                    moveDirection = moveDirection.normalized;
                    //playerMove.transform.rotation = Quaternion.LookRotation (moveDirection);
                    physics.TurnTo(moveDirection);
                    var movement = moveDirection * walkSpeed;
                    physics.MoveSpeed(movement);
                }
                if (passTime >= animation [attackAniName].length * 0.8f / animation [attackAniName].speed)
                {
                    break;
                }
                passTime += Time.deltaTime;

                var vhValue = Mathf.Abs(v) + Mathf.Abs(h);
                if (hitYet && vhValue > 0.2f)
                {
                    //stopAttack = true;
                    break;
                }

                yield return null;
            } while(!quit);
			
            Log.Ani("Animation is Playing stop " + attackAniName);
            skillStateMachine.Stop();
        }

        public override IEnumerator RunLogic()
        {
            Log.AI("Run HumanCombat Logic");
            bool first = true;

            while (!quit)
            {
                attackAniName = GetAttackAniName(); 

                var realAttackTime = activeSkill.skillData.AttackAniTime / GetAttr().GetSpeedCoff();
                var rate = GetAttr().GetComponent<Animation>() [attackAniName].length / realAttackTime;
                if (first)
                {
                    PlayAni(attackAniName, rate, WrapMode.Once);
                    first = false;
                } else
                {
                    SetAni(attackAniName, rate, WrapMode.Once);
                }
                var rd = Random.Range(1, 10);
                if (rd <= 3)
                {
                    BackgroundSound.Instance.PlayEffect("destroyereffort" + rd);
                }


                Log.Ani("Do ule Press Time " + attackAniName + "  " + pressAttack + " " + attackPressTime + " " + Time.time + " " + WindowTime);
                yield return GetAttr().StartCoroutine(WaitForAttackAnimation(GetAttr().GetComponent<Animation>()));

                //stopAttack = false;
                if (pressAttack && ((Time.time - attackPressTime) < WindowTime))
                {
                    Log.AI("Press Attack Again");
                    pressAttack = false;
                } else
                {
                    break;
                }
            }
            Log.AI("Combat Over ");
            aiCharacter.ChangeState(AIStateEnum.IDLE);
        }
    }

}
using UnityEngine;
using System.Collections;

namespace MyLib
{
    public enum AIStateEnum
    {
        INVALID = -1,
        IDLE,
        COMBAT,
        DEAD,
        MOVE,

        FLEE,
        KNOCK_BACK,

        CAST_SKILL,
        Stunned,

        JUMP,

        MOVE_SHOOT,
        STOP,
    }

    public class AIState
    {
        protected AICharacter aiCharacter;
        protected bool quit = false;

        public void SetChar(AICharacter a)
        {
            aiCharacter = a;
        }

        public AIStateEnum type = AIStateEnum.INVALID;
        /// <summary>
        ///构成一个Idle组成的树状结构
        /// Idle对应的状态有自己的层 
        /// </summary>
        public int layer = 0;

        public AIState()
        {
		
        }

        /// <summary>
        ///重载的时候 子类需要在while中判定是否quit状态
        /// </summary>
        public virtual IEnumerator RunLogic()
        {
            while (!quit)
            {
                if (CheckEvent())
                {
                    break;
                }
                yield return null;
            }
        }

        public static AIState CreateState(AIStateEnum s)
        {
            switch (s)
            {
                case AIStateEnum.IDLE:
                    return new IdleState();
                case AIStateEnum.COMBAT:
                    return new CombatState();
                case AIStateEnum.DEAD:
                    return new DeadState();
                case AIStateEnum.MOVE:
                    return new MoveState();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 重载的时候 子类 需要 执行父类的enterState exitState 函数
        /// </summary>
        public virtual void EnterState()
        {
            quit = false;
        }

        /// <summary>
        /// 重载的时候 子类 需要 执行父类的enterState exitState 函数
        /// </summary>
        public virtual void ExitState()
        {
            quit = true;
        }

        public virtual bool CheckNextState(AIStateEnum next)
        {
            return true;
        }

        public virtual bool CanChangeState(AIStateEnum next) {
            return CheckNextState(next);
        }

        protected void PlayAni(string name, float speed, WrapMode wm)
        {
            aiCharacter.PlayAni(name, speed, wm);
        }

        protected void SetAni(string name, float speed, WrapMode wm)
        {
            aiCharacter.SetAni(name, speed, wm);
        }

        protected bool CheckAni(string name)
        {
            return aiCharacter.GetAttr().GetComponent<Animation>().GetClip(name) != null;
        }

        protected void SetAttrState(CharacterState s)
        {
            aiCharacter.GetAttr()._characterState = s;
        }

        NpcAttribute attr;

        protected NpcAttribute GetAttr()
        {
            if (attr == null)
            {
                attr = aiCharacter.GetAttr();
            }
            return attr;
        }

        protected MyAnimationEvent GetEvent()
        {
            return GetAttr().GetComponent<MyAnimationEvent>();
        }

        protected SkillInfoComponent GetSkill()
        {
            return GetAttr().GetComponent<SkillInfoComponent>();
        }



        protected CommonAI GetCommonAI()
        {
            return GetAttr().GetComponent<CommonAI>();
        }

        protected bool CheckFlee()
        {
            if (GetEvent().fleeEvent && (Time.time - GetEvent().fleeTime) < 0.1f)
            {
                GetEvent().fleeEvent = false;
                return true;
            } else
            {
                GetEvent().fleeEvent = false;
            }
            return false;
        }

        bool DoDamage()
        {
            var rd = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffect("bladeimp" + rd);
            var damage = GetEvent().FetchDamage();
            var shit = false;
            bool isCritical = false;
            foreach (MyAnimationEvent.DamageData d in damage)
            {
                Log.Important("Dohurt damage " + d.Damage);
                if (d.isCritical)
                {
                    isCritical = true;
                }
                if (NetDebug.netDebug.IsWuDi && aiCharacter.attribute.gameObject == ObjectManager.objectManager.GetMyPlayer())
                {
                } else
                {
                    GetAttr().DoHurt(d.Damage, d.isCritical);
                }
                if (d.ShowHit)
                {
                    shit = true;
                }
            }
            GetEvent().ClearDamage();

            if (shit)
            {
                if (isCritical)
                {
                    var criticalEffect = GameObject.Instantiate(Resources.Load<GameObject>("particles/criticalHit")) as GameObject;
                    criticalEffect.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                    criticalEffect.transform.localPosition = GetAttr().transform.localPosition + Vector3.up;
                    criticalEffect.transform.localRotation = Quaternion.identity;
                    criticalEffect.transform.localScale = Vector3.one;
                    criticalEffect.AddMissingComponent<RemoveSelf>();
                } else
                {
                    var bloodBomb = Resources.Load<GameObject>("particles/monsters/player/bloodHit");
                    GameObject g = GameObject.Instantiate(bloodBomb) as GameObject;
                    g.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                    g.transform.localPosition = GetAttr().transform.localPosition + new Vector3(0, 1, 0);
                    g.transform.localRotation = Quaternion.identity;
                    g.transform.localScale = Vector3.one;
                }
                var swordHit = Resources.Load<GameObject>("particles/swordhit");
                GameObject g2 = GameObject.Instantiate(swordHit) as GameObject;
                g2.transform.parent = SaveGame.saveGame.EffectMainNode.transform;
                g2.transform.position = GetAttr().transform.position + new Vector3(0, 1, 0);
            }
            return shit;
        }

        //只检测是否受到攻击 不 显示状态变化
        bool CheckOnHit()
        {
            if (GetAttr().IsDead)
                return false;
			
            if (GetEvent().onHit)
            {
                DoDamage();
                return true;
            }
            return false;
        }

        protected bool CheckBaseEvent()
        {
            CheckOnHit();
            if (GetAttr().CheckDead())
            {
                return aiCharacter.ChangeState(AIStateEnum.DEAD);
            }
            if (GetEvent().timeToDead)
            {
                return aiCharacter.ChangeState(AIStateEnum.DEAD);
            }
            if (GetEvent().KnockBack)
            {
                GetEvent().KnockBack = false;
                return aiCharacter.ChangeState(AIStateEnum.KNOCK_BACK);	
            }
            return false;
        }

        protected MyAnimationEvent.Message lastMsg;

        protected void ClearEvent()
        {
            GetEvent().ClearMsg();
        }

        //TODO:检测一些事件 然后进行状态切换
        //获得对应注册哪些事件， 就检测哪些事件？
        //只有状态切换成功才回 CheckEvent 返回true
        protected bool CheckEvent()
        {
            if (CheckBaseEvent())
            {
                return true;
            }
            var msg = GetEvent().NextMsg();
            lastMsg = msg;
            if (msg != null)
            {
                Log.AI("CheckEventIs " + msg.type);
                if (msg.type == MyAnimationEvent.MsgType.IDLE)
                {
                    return aiCharacter.ChangeState(AIStateEnum.IDLE);
                } else if (msg.type == MyAnimationEvent.MsgType.DoSkill)
                {
                    var isBaseAttack = GetSkill().IsDefaultSkill(msg.skillData);
                    Log.AI("CheckCastSkill " + isBaseAttack);
                    if (isBaseAttack)
                    {
                        Log.AI("CheckAttack " + msg.type);
                        var skillPart = GetSkill();
                        skillPart.SetDefaultActive();
                        return aiCharacter.ChangeState(AIStateEnum.COMBAT);
                        
                    } else
                    {
                        Log.AI("Enter CastSkill");
                        var skillPart = GetSkill();
                        skillPart.SetActiveSkill(msg.skillData);
                        //技能动作为空
                        if (string.IsNullOrEmpty(msg.skillData.AnimationName))
                        {
                            SkillLogic.UseSkill(GetAttr());
                            return false;
                        } else
                        {
                            return aiCharacter.ChangeState(AIStateEnum.CAST_SKILL);
                        }
                    }
                } else if (msg.type == MyAnimationEvent.MsgType.STUNNED)
                {
                    return aiCharacter.ChangeState(AIStateEnum.Stunned);
                } else if (msg.type == MyAnimationEvent.MsgType.EXIT_STUNNED)
                {
                    return aiCharacter.ChangeState(AIStateEnum.IDLE);
                } else if (msg.type == MyAnimationEvent.MsgType.DEAD)
                {
                    return aiCharacter.ChangeState(AIStateEnum.DEAD);
                } else if (msg.type == MyAnimationEvent.MsgType.JUMP)
                {
                    Log.AI("EnterJumpStateNow");
                    return aiCharacter.ChangeState(AIStateEnum.JUMP);
                } else
                {
                    return CheckEventOverride(msg);
                }

            }
            return false;
            //return CheckAttackEvent ();
        }

        protected virtual bool CheckEventOverride(MyAnimationEvent.Message msg)
        {
            Log.AI("DefaultEvent " + msg.type);
            return false;
        }

    }

    public class IdleState  : AIState
    {
        public IdleState()
        {
            type = AIStateEnum.IDLE;
        }

    }

    public class CombatState : AIState
    {
        public CombatState()
        {
            type = AIStateEnum.COMBAT;
        }
    }

    public class MoveState : AIState
    {
        public MoveState()
        {
            type = AIStateEnum.MOVE;
        }

    }

    public class DeadState : AIState
    {
        public DeadState()
        {
            type = AIStateEnum.DEAD;
        }

        public override void EnterState()
        {
            base.EnterState();
            MyEventSystem.myEventSystem.PushLocalEvent(GetAttr().GetLocalId(), MyEvent.EventType.MeDead);
        }

        public override bool CheckNextState(AIStateEnum next)
        {
            return false;
        }
    }

    public class FleeState : AIState
    {
        public FleeState()
        {
            type = AIStateEnum.FLEE;
        }
    }

    public class KnockBackState : AIState
    {
        public KnockBackState()
        {
            type = AIStateEnum.KNOCK_BACK;
        }
    }

    public class SkillState : AIState
    {
        public SkillState()
        {
            type = AIStateEnum.CAST_SKILL;
        }

        public override bool CheckNextState(AIStateEnum next)
        {
            if (next == AIStateEnum.CAST_SKILL)
            {
                return false;
            }
            if (next == AIStateEnum.COMBAT)
            {
                return false;
            }
            return base.CheckNextState(next);
        }
    }

    public class StunnedState : AIState
    {
        public StunnedState()
        {
            type = AIStateEnum.Stunned;
        }

        public override bool CheckNextState(AIStateEnum next)
        {
            if (next == AIStateEnum.DEAD)
            {
                return true;
            }
            if (next == AIStateEnum.IDLE)
            {
                if (lastMsg != null)
                {
                    if (lastMsg.type == MyAnimationEvent.MsgType.EXIT_STUNNED)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 避免跳跃过程中死亡状态突然出现怎么处理？
    /// 不要切换状态下次再检测CheckEvent 即可
    /// Message 会丢弃 但是 常驻的state不会 
    /// </summary>
    public class JumpState : AIState
    {
        public JumpState()
        {
            type = AIStateEnum.JUMP;
        }

        public override bool CheckNextState(AIStateEnum next)
        {
            if (next == AIStateEnum.IDLE)
            {
                return true;
            }
            return false;
        }
    }

    public class MoveShootState : AIState
    {
        public MoveShootState()
        {
            type = AIStateEnum.MOVE_SHOOT;
        }
    }
}
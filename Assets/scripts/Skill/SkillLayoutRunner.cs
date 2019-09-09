using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SkillLayoutRunner : MonoBehaviour
    {
        public SkillStateMachine stateMachine
        {
            get;
            set;
        }

        public SkillDataConfig.EventItem Event
        {
            get;
            set;
        }
        public MyEvent triggerEvent;

        public void DoDamage(GameObject g)
        {
            Log.AI("SkillLayout DoDamage " + Event.affix.effectType);
            var attr = NetworkUtil.GetAttr(g);
            if (Event.affix.effectType != Affix.EffectType.None && Event.affix.target == Affix.TargetType.Enemy)
            {
                //Buff目标是本地控制
                //通过ID来控制复制信息 实体都要有ID才便于复制
                //if(stateMachine.attacker.GetComponent<NpcAttribute>().IsMine()) {

                if(attr.IsMine())
                {
                    attr.GetComponent<BuffComponent>().AddBuff(Event.affix, stateMachine.attacker.transform.position);
                    NetDateInterface.FastMoveAndPos();
                    NetDateInterface.FastAddBuff(Event.affix, stateMachine.attacker, attr.gameObject, stateMachine.skillFullData.skillId, Event.EvtId);
                }
            }
            stateMachine.DoDamage(attr.gameObject);
        }

        //玩家先在处于一个Skill状态 玩家先在处于一个Skill状态 技能时间1s钟 玩家才能结束状态
        public bool MoveOwner(Vector3 position, float speed)
        {
            stateMachine.attacker.GetComponent<PhysicComponent>().SkillMove(position, speed);
            return true;
        }

        IEnumerator FollowAttacker()
        {
            Log.AI("attach Particle to player");
            //transform.parent = stateMachine.attacker.transform;
            var attacker = stateMachine.attacker.transform;
            transform.localScale = Vector3.one;
            while (attacker != null)
            {
                transform.localPosition = attacker.transform.localPosition;
                transform.localRotation = attacker.transform.localRotation;
                yield return null;
            }
        }

        /// <summary>
        ///显示粒子效果的时候出现在特定位置 
        /// </summary>
        /// <returns>The particle.</returns>
        IEnumerator ShowParticle()
        {
            var skillConfig = Event.layout.GetComponent<SkillLayoutConfig>();
            var particle = Event.layout.GetComponent<SkillLayoutConfig>().particle;
            //NGUITools.AddMissingComponent<RemoveSelf> (particle);
            Log.Sys("Init Particle for skill Layout " + particle + " " + skillConfig + " bone name " + skillConfig.boneName);
            if (particle != null)
            {
                if (skillConfig.delayTime > 0)
                {
                    yield return new WaitForSeconds(skillConfig.delayTime);
                }

                var g = GameObject.Instantiate(particle) as GameObject;
                var xft = g.GetComponent<XffectComponent>();
                //Wait To Show Particle Why?
                xft.enabled = false;
                NGUITools.AddMissingComponent<RemoveSelf>(g);
                
                if (skillConfig.boneName != "")
                {
                    Log.Sys("add particle to bone " + skillConfig.boneName);
                    //g.transform.parent = transform;
                    var par = Util.FindChildRecursive(stateMachine.attacker.transform, skillConfig.boneName);
                    if (par == null)
                    {
                        par = stateMachine.attacker.transform;
                    }
                    
                    //g.transform.parent =  
                    g.transform.localPosition = skillConfig.Position + par.transform.position;
                    g.transform.localRotation = Quaternion.identity;
                    g.transform.localScale = Vector3.one;
                    
                } else
                {
                    Log.Ani("Particle TargetPos " + Event.TargetPos);
                    if (Event.TargetPos)
                    {
                        if (stateMachine.target != null)
                        {
                            stateMachine.MarkPos = stateMachine.target.transform.position;
                            g.transform.position = stateMachine.target.transform.position + skillConfig.Position;
                            g.transform.localRotation = Quaternion.identity;
                            g.transform.localScale = Vector3.one;
                        }
                    } else if (Event.AttachToTarget)
                    {
                        if (stateMachine.target != null)
                        {
                            g.transform.parent = stateMachine.target.transform;
                            g.transform.localPosition = skillConfig.Position;
                            g.transform.localRotation = Quaternion.identity;
                            g.transform.localScale = Vector3.one;
                        }
                    }else if(Event.UseMarkPos) {
                        g.transform.position = stateMachine.MarkPos + skillConfig.Position;
                        g.transform.localRotation = Quaternion.identity;
                        g.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        g.transform.parent = transform;
                        g.transform.localPosition = skillConfig.Position;
                        g.transform.localRotation = Quaternion.identity;
                        g.transform.localScale = Vector3.one;
                    }
                }

                //火焰哨兵的激光直接给目标造成伤害
                if (Event.SetBeamTarget)
                {
                    Log.AI("SetBeamTarget is " + stateMachine.target.transform.position);
                    var bt = Util.FindChildrecursive<BeamTarget>(g.transform);
                    if(bt != null) {
                        bt.transform.position = stateMachine.target.transform.position + Event.BeamOffset;
                    }
                    BeamTargetPos = stateMachine.target.transform.position + Event.BeamOffset;
                }
                StartCoroutine(EnableXft(xft));
            }
        }

        public Vector3 BeamTargetPos = Vector3.zero;

        void Start()
        {
            if (Event.attaches)
            {
                StartCoroutine(FollowAttacker());
            }

            if (Event.affix.effectType != Affix.EffectType.None && Event.affix.target == Affix.TargetType.Self)
            {
                if (stateMachine.attacker != null && stateMachine.attacker.GetComponent<NpcAttribute>().IsMine())
                {
                    stateMachine.attacker.GetComponent<BuffComponent>().AddBuff(Event.affix);
                    NetDateInterface.FastMoveAndPos();
                    NetDateInterface.FastAddBuff(Event.affix, stateMachine.attacker, stateMachine.attacker, stateMachine.skillFullData.skillId, Event.EvtId);
                }
            }
            

            var skillConfig = Event.layout.GetComponent<SkillLayoutConfig>();
            if (skillConfig != null)
            {
                StartCoroutine(ShowParticle());
            }
        }

        IEnumerator EnableXft(XffectComponent xft)
        {
            yield return null;
            xft.enabled = true;
        }
    }
}

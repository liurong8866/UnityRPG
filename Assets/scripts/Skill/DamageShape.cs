using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
    /// <summary>
    /// 技能的伤害碰撞模型
    /// </summary>
    public class DamageShape : MonoBehaviour
    {
        /// <summary>
        /// 技能伤害判定形状
        /// 球形和角度， 线性通过子弹模拟 
        /// </summary>
        public enum Shape
        {
            Sphere,
            Angle,
            Line,
        }

        public Shape shape = Shape.Sphere;
        //正负多少角度攻击
        //普通物理 攻击 以及 火焰陷阱的 攻击使用了这个 Angle
        public float angle = 0;
        //< 90

        /// <summary>
        /// DamageShape 的父亲，技能层运行器 
        /// </summary>
        SkillLayoutRunner runner;
        //伤害持续的时间通过enable确定
        //如果是一开始 DamageShape 就要生效，那么就设置Enable 为true，否则用动画Timeline 控制enable什么时候为true

        //火焰陷阱喷出的火焰 只造成一次伤害，伤害之后，则失效，因此默认DamageShape enable为true 且e 为true，
        /// <summary>
        /// enable状态才造成伤害 
        /// </summary>
        public bool enable;
        /// <summary>
        /// 球体的攻击半径 
        /// </summary>
        public float radius = 2f;
        /// <summary>
        /// 用于角度判定的变量 
        /// </summary>
        float cosAngle;

        //是否只产生一次伤害  一次性攻击和持续性攻击 火焰灼烧技能持续
        public bool Once = false;
        bool damageYet = false;
        /// <summary>
        /// 记录当前已经攻击过的对象 
        /// </summary>
        HashSet<GameObject> hurtEnemy = new HashSet<GameObject>();
        Vector3 InitPosition;

        //向前冲击技能需要 DamageShape 和玩家的位置重合在一起  冲击技能将会判定伤害根据玩家位置  否则 根据DamageShape自身位置判定伤害
        public bool SyncWithPlayer = false;

        //是否造成伤害 不造成伤害只拖动玩家移动
        public bool NoHurt = false;

        //冲击移动速度 冲刺的最终距离
        public float speed = 6;
        /// <summary>
        /// 防止玩家被拖动多次 
        /// </summary>
        bool enableYet = false;
        /// <summary>
        /// 期望移动的距离 
        /// </summary>
        public float Distance = 6;
        Vector3 targetPos;
        /// <summary>
        /// 碰撞体延迟作用的时间 
        /// </summary>
        public float delayTime = 0;
        private  float passTime = 0;

        void Awake()
        {
            passTime = 0;
        }

        /// <summary>
        /// Start时候获取Runner
        /// 角度
        /// 如果是冲击技能则 初始化伤害体初始位置和最终位置 
        /// </summary>
        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            cosAngle = Mathf.Cos(Mathf.Deg2Rad * angle);
            if (runner != null && runner.Event.attachOwner)
            {
                InitPosition = runner.transform.position;
                targetPos = InitPosition + transform.forward * Distance;
            }
        }

    
        /// <summary>
        /// 延迟等待
        /// 判定是否是一次性伤害技能 还是持续性伤害
        /// 
        /// 开始判定伤害，技能没有结束
        /// 无伤害只移动对象
        /// 
        ///  
        /// </summary>
        void Update()
        {
            passTime += Time.deltaTime;
            if (passTime < delayTime)
            {
                return;
            }
            if (Once && damageYet)
            {
                return;
            }
            if (!runner || !runner.stateMachine || !runner.stateMachine.attacker)
            {
                return;
            }

            if (enable && !runner.stateMachine.isStop)
            {
                if (!NoHurt)
                {
                    //物理碰撞检测 玩家附近 或者 碰撞体附近的单位
                    Collider[] hitColliders;
                    if (SyncWithPlayer)
                    {
                        hitColliders = Physics.OverlapSphere(runner.stateMachine.attacker.transform.position, radius, SkillDamageCaculate.GetDamageLayer());
                    } else
                    {
                        hitColliders = Physics.OverlapSphere(transform.position, radius, SkillDamageCaculate.GetDamageLayer());
                    }

                    for (int i = 0; i < hitColliders.Length; i++)
                    {
                        if (SkillLogic.IsEnemy(runner.stateMachine.attacker, hitColliders [i].gameObject))
                        {
                            if (!hurtEnemy.Contains(hitColliders [i].gameObject))
                            {
                                if (shape == Shape.Sphere)
                                {
                                    DoDamage(hitColliders [i].gameObject);
                                    hurtEnemy.Add(hitColliders [i].gameObject);
                                } else if (shape == Shape.Angle)
                                {
                                    Log.AI("DamageHit " + runner.stateMachine.name + " " + hitColliders [i].name);
                                    var dir = hitColliders [i].gameObject.transform.position - transform.position;
                                    var cos = Vector3.Dot(dir.normalized, transform.forward);
                                    if (cos > cosAngle)
                                    {
                                        DoDamage(hitColliders [i].gameObject);
                                        hurtEnemy.Add(hitColliders [i].gameObject);
                                    }
                                }
                            }
                        }
                    }
                    Log.AI("Check Damage Shape " + runner.stateMachine.name);
                    damageYet = true;
                }

                ///DamageShape 开始移动玩家 同时也移动玩家 
                if (runner != null && runner.Event.attachOwner)
                {
                    Log.Sys("Move Attack With DamageShape");
                    if (!enableYet)
                    {
                        StartCoroutine(MoveOwner());
                    }
                }
                enableYet = true;
            }
        }


        /// <summary>
        /// 进入技能移动状态 停止玩家手动操控
        /// </summary>
        /// <returns>The owner.</returns>
        IEnumerator MoveOwner()
        {
            Log.Sys("enter Move State");
            var ret = runner.stateMachine.attacker.GetComponent<PhysicComponent>().EnterSkillMoveState();
            if (!ret)
            {
                yield break;
            }

            float diff = 0;
            float halfDist = Distance / 2.0f;

            //小于一半距离 则加速流程
            //需要移动到目标位置 targetPos 控制移动速度
            do
            {
                if (!runner || !runner.stateMachine || !runner.stateMachine.attacker)
                {
                    break;
                }

                diff = (targetPos - runner.stateMachine.attacker.transform.position).sqrMagnitude;
                var newSpeed = speed;
                if (diff < halfDist)
                {
                    newSpeed = diff / halfDist * speed;
                }

                runner.MoveOwner(targetPos, newSpeed);
                yield return null;
            } while(diff > 0.1f && !runner.stateMachine.isStop && enable);

            Log.Sys("ExitSkillMove ");
            runner.stateMachine.attacker.GetComponent<PhysicComponent>().ExitSkillMove();
        }

        /*
         * 伤害计算过程
            1：伤害对象判定
            2：伤害数值确定
         */
        void DoDamage(GameObject g)
        {
            Log.Sys("DoDamage: " + g + " runner " + runner);
            runner.DoDamage(g);
        }

    }

}
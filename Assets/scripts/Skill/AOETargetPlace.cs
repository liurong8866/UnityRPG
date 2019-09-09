﻿using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 在目标位置产生一个AOE技能伤害目标位置附近单位，类似于子弹飞射，最后目标位置碰撞爆炸 
    /// </summary>
    public class AOETargetPlace : MonoBehaviour
    {
        public GameObject DieParticle;
        public float AOERadius;

        SkillLayoutRunner runner;
        public Vector3 ParticlePos;
        public float WaitTime = 0.5f; //等待造成伤害的时间
        //当前位置来做AOE
        public bool AOEHere = false;
        // Use this for initialization
        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            if(runner.triggerEvent != null && runner.triggerEvent.type == MyEvent.EventType.EventMissileDie) {
                transform.position = runner.triggerEvent.missile.transform.position;
            }

            var attacker = runner.stateMachine.attacker; 
            StartCoroutine(WaitExplosive());


        }

        /// <summary>
        /// 根据BeamTargetPos 激光瞄准位置
        /// 或者MarkPos 标记位置来选择攻击目标 
        /// </summary>
        /// <returns>The explosive.</returns>
        IEnumerator WaitExplosive() {
            yield return new WaitForSeconds(WaitTime);
            var tarPos = Vector3.zero;
            if(AOEHere) {
                tarPos = transform.position;
            }
            else if(runner.BeamTargetPos != Vector3.zero){
                tarPos = runner.BeamTargetPos;
            }else if(runner.stateMachine.MarkPos != Vector3.zero) {
                tarPos = runner.stateMachine.MarkPos;
            }
            if(tarPos != Vector3.zero)
            {
                transform.position = tarPos;
                if(DieParticle != null) {
                    GameObject g = Instantiate (DieParticle) as GameObject;
                    NGUITools.AddMissingComponent<RemoveSelf> (g);
                    g.transform.parent = ObjectManager.objectManager.transform;
                    g.transform.position = runner.BeamTargetPos+ParticlePos;
                }

                Collider[] col = Physics.OverlapSphere (transform.position, AOERadius, SkillDamageCaculate.GetDamageLayer());
                foreach (Collider c in col) {
                    DoDamage (c);
                }
            }else {
                Debug.LogError("BeamTargetPos Is NULL");
            }
        }

        /// <summary>
        ///子弹伤害计算也交给skillLayoutRunner执行 
        /// </summary>
        /// <param name="other">Other.</param>
        void DoDamage(Collider other){
            if (SkillLogic.IsEnemy(runner.stateMachine.attacker, other.gameObject)) {
                var skillData = runner.stateMachine.skillFullData.skillData;
                if(!string.IsNullOrEmpty(skillData.HitSound)) {
                    BackgroundSound.Instance.PlayEffect(skillData.HitSound);
                }
                if(runner != null) {
                    runner.DoDamage(other.gameObject);
                }
            }
        }
       
    }
}

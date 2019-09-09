using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    ///在攻击目标身上产生一个粒子效果 
    /// </summary>
    public class TargetBindParticle : MonoBehaviour
    {
        public GameObject Particle;
        public Vector3 ParticlePos;

        SkillLayoutRunner runner;
        void Start()
        {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            var attacker = runner.stateMachine.attacker; 
        
            if(runner.BeamTargetPos != Vector3.zero){
                if(Particle != null) {
                    GameObject g = Instantiate (Particle) as GameObject;
                    NGUITools.AddMissingComponent<RemoveSelf> (g);
                    g.transform.parent = ObjectManager.objectManager.transform;
                    g.transform.position = runner.BeamTargetPos+ParticlePos;
                }
            }
        }


    }

}
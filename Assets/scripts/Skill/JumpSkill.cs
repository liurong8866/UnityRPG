using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class JumpSkill : MonoBehaviour
    {
        SkillLayoutRunner runner;
        void Start() {
            runner = transform.parent.GetComponent<SkillLayoutRunner>();
            var evt = runner.stateMachine.attacker.GetComponent<MyAnimationEvent>();
            evt.EnterJump();

        }
    }
}
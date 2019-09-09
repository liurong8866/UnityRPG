using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class SummonCombat :CombatState
    {
        float FastRotateSpeed = 10;
        public override void EnterState()
        {
            base.EnterState();
        }

        IEnumerator CastSkill()
        {
            var targetPlayer = ObjectManager.objectManager.GetMyPlayer();
            GetAttr().GetComponent<SkillInfoComponent>().SetRandomActive();
            var activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();
            var skillStateMachine = SkillLogic.CreateSkillStateMachine(GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position, targetPlayer);
            Log.AI("Skill SetAni " + activeSkill.skillData.AnimationName);

            var realAttackTime = activeSkill.skillData.AttackAniTime / GetAttr().GetSpeedCoff();
            var rate = GetAttr().GetComponent<Animation>() [activeSkill.skillData.AnimationName].length / realAttackTime;
            SetAni(activeSkill.skillData.AnimationName, rate, WrapMode.Once);
            var physic = GetAttr().GetComponent<PhysicComponent>();
            while (GetAttr().GetComponent<Animation>().isPlaying && !quit)
            {
                if (CheckEvent())
                {
                    break;
                }

                //自动向目标旋转
                Vector3 dir = targetPlayer.transform.position - GetAttr().transform.position;
                dir.y = 0;
                var newDir = Vector3.Slerp(GetAttr().transform.forward, dir, Time.deltaTime * FastRotateSpeed);
                physic.TurnTo(newDir);
                yield return null;
            }
            skillStateMachine.Stop();
        }

        public override IEnumerator RunLogic()
        {
            while (!quit)
            {
                yield return GetAttr().StartCoroutine(CastSkill());
                yield return new WaitForSeconds(1);
                var summons = ObjectManager.objectManager.GetSummons(GetAttr().GetLocalId());
                while(summons.Count >= 10 && !quit) {
                    if(CheckEvent()){
                        break;
                    }
                    yield return new WaitForSeconds(2);
                    summons = ObjectManager.objectManager.GetSummons(GetAttr().GetLocalId());
                }

            }
        }

    }
}

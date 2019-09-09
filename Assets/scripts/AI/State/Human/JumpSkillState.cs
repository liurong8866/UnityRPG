using UnityEngine;
using System.Collections;

namespace MyLib
{
    public class JumpSkillState : AIState
    {

        string GetAttackAniName()
        {
            return "rslash_1";
        }

        IEnumerator WaitForAttackAnimation(Animation animation)
        {
            var rd = Random.Range(1, 3);
            BackgroundSound.Instance.PlayEffect("onehandswinglarge" + rd);
            var skillStateMachine = SkillLogic.CreateSkillStateMachine(GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position);
            Log.AI("Wait For Combat Animation");
            float passTime = 0;
            do
            {
                if (passTime >= animation [GetAttackAniName()].length * 0.8f / animation [GetAttackAniName()].speed)
                {
                    break;
                }
                passTime += Time.deltaTime;

                yield return null;
            } while(!quit);
            
            Log.Ani("Animation is Playing stop ");
            skillStateMachine.Stop();
        }

        SkillFullInfo activeSkill = null;

        public override IEnumerator RunLogic()
        {
            activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();
            var attackAniName = GetAttackAniName(); 
            var realAttackTime = activeSkill.skillData.AttackAniTime / GetAttr().GetSpeedCoff();
            var rate = GetAttr().GetComponent<Animation>() [attackAniName].length / realAttackTime;
            PlayAni(attackAniName, rate, WrapMode.Once);
            yield return GetAttr().StartCoroutine(WaitForAttackAnimation(GetAttr().GetComponent<Animation>()));

            aiCharacter.SetRun();
            //等动画彻底播放完成
            yield return new WaitForSeconds(0.1f);
        }
    }

}
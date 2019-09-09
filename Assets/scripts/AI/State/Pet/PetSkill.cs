using UnityEngine;
using System.Collections;
 
namespace MyLib
{
    public class PetSkill : SkillState
    {
        SkillStateMachine skillStateMachine;
        SkillFullInfo activeSkill;

        public override void EnterState()
        {
            base.EnterState();
            Log.AI("Enter Pet Skill State ");
            //启动技能状态机 启动动画
            activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();
			skillStateMachine = SkillLogic.CreateSkillStateMachine (GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position, null);

            SetAni(activeSkill.skillData.AnimationName, 1f, WrapMode.Once);
        }

        public override IEnumerator RunLogic()
        {
            Log.AI("Check Animation " + GetAttr().GetComponent<Animation>().IsPlaying(activeSkill.skillData.AnimationName));
            float passTime = 0;
            var animation = GetAttr().GetComponent<Animation>();
            var attackAniName = activeSkill.skillData.AnimationName;

            while (!quit)
            {
                
                if (CheckEvent())
                {
                    yield break;
                }
                if (skillStateMachine.skillDataConfig.animationLoop )
                {
                    if(passTime >= skillStateMachine.skillDataConfig.attackDuration) {
                        break;
                    }
                } else
                {
                    if (passTime >= animation [attackAniName].length * 0.8f / animation [attackAniName].speed)
                    {
                        break;
                    }
                }
                passTime += Time.deltaTime;
                yield return null;
            }
            Log.AI("Stop SkillState ");
            skillStateMachine.Stop();
            aiCharacter.ChangeState(AIStateEnum.IDLE);
        }
    }
}

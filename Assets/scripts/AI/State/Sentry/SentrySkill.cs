using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class SentrySkill : SkillState
	{
		SkillFullInfo activeSkill;
		SkillStateMachine skillStateMachine;
		public override void EnterState()
		{
			base.EnterState();
			Log.AI("sentry Enter Skill State ");
			activeSkill = GetAttr().GetComponent<SkillInfoComponent>().GetActiveSkill();
			skillStateMachine = SkillLogic.CreateSkillStateMachine (GetAttr().gameObject, activeSkill.skillData, GetAttr().transform.position, aiCharacter.GetEnemy());

			SetAni(activeSkill.skillData.AnimationName, 1f, WrapMode.Once);
			if (GetAttr ().ObjUnitData.AttachToMaster) {
				GetAttr ().StartCoroutine (TraceMaster());
			}
		}
		IEnumerator TraceMaster() {
			while (!quit) {
				//if (GetAttr ().ObjUnitData.AttachToMaster) {
				var transform = GetAttr ().transform;
				transform.position = Vector3.Lerp (transform.position, GetAttr ().GetOwner ().transform.position, Time.deltaTime * 3);
				transform.rotation = Quaternion.Lerp (transform.rotation, GetAttr ().GetOwner ().transform.rotation, Time.deltaTime * 3);	
				//}
				
				yield return null;
			}
			Log.AI ("quit Trace master ");
		}

		public override IEnumerator RunLogic()
		{
			Log.AI("Check Animation sentry " + GetAttr().GetComponent<Animation>().IsPlaying(activeSkill.skillData.AnimationName));
			float passTime = 0;
			//var animation = GetAttr().animation;
			//var attackAniName = activeSkill.skillData.AnimationName;
			/*
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
			*/
			while (!quit && passTime < 2) {
				if(CheckEvent()) {
					yield break;
				}
				passTime += Time.deltaTime;
				yield return null;
			}
			Log.AI("Stop SkillState sentry "+passTime);
			skillStateMachine.Stop();
			aiCharacter.ChangeState(AIStateEnum.IDLE);
		}

	}
}

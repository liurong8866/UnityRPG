using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class MonsterKnockBack : KnockBackState
	{
		float KnockBackTime = 0.6f;
		float KnockBackSpeed = 6;
		float StopKnockTime = 0.4f;

		public override void EnterState ()
		{
			base.EnterState ();
			aiCharacter.SetIdle ();
		}
		public override void ExitState ()
		{
			base.ExitState ();
		}
		public override bool CheckNextState (AIStateEnum next)
		{
			if (next == AIStateEnum.KNOCK_BACK) {
				return false;
			}
			return base.CheckNextState (next);
		}
		public override IEnumerator RunLogic ()
		{
			var physic = GetAttr ().GetComponent<PhysicComponent> ();
            Vector3 moveDirection = GetAttr().transform.position - GetEvent().KnockWhoPos;
			moveDirection.y = 0;

			float curFlySpeed = 0;
			float passTime = 0;
			while(passTime < KnockBackTime) {
				curFlySpeed = Mathf.Lerp(curFlySpeed, KnockBackSpeed, 5*Time.deltaTime);
				var movement = moveDirection * curFlySpeed;
				physic.MoveSpeed(movement);
				passTime += Time.deltaTime;
				yield return null;

				if(CheckEvent()) {
					yield break;
				}
			}
			
			float stopTime = 0;
			while(stopTime < StopKnockTime) {
				curFlySpeed = Mathf.Lerp(curFlySpeed, 0, 5*Time.deltaTime);
				var movement = moveDirection * curFlySpeed;
				physic.MoveSpeed(movement);

				stopTime += Time.deltaTime;
				yield return null;
				if(CheckEvent()) {
					yield break;
				}
			}

			aiCharacter.ChangeState (AIStateEnum.IDLE);
		}
	}
}

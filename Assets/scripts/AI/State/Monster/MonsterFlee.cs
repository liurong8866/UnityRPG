using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class MonsterFlee : FleeState
	{
		float FastRotateSpeed = 10;
		float RunSpeed = 5;

		public override IEnumerator RunLogic ()
		{
			yield return GetAttr ().StartCoroutine (FleeCoroutine());
		}

		IEnumerator FleeCoroutine() {
			var physic = GetAttr ().GetComponent<PhysicComponent> ();
			SetAttrState (CharacterState.Flee);
			var targetPlayer = ObjectManager.objectManager.GetMyPlayer();
			var dir = GetAttr().transform.position - targetPlayer.transform.position;
			dir.y = 0;
			dir.Normalize ();
			aiCharacter.SetRun ();

			//Rotate To Dir 
			//Then Start To Run Speed For Time 1s
			var rotation = Quaternion.LookRotation(dir);
			float rotTime = 0;
			while (rotTime < 0.1f) {
				if(CheckEvent()) {
					yield break;
				}

				GetAttr().transform.rotation = Quaternion.Slerp(GetAttr().transform.rotation, rotation, Mathf.Min(1, Time.deltaTime*FastRotateSpeed));
				rotTime += Time.deltaTime;
				yield return null;
			}

			float runTime = 0;
			while(runTime < 2f) {
				if(CheckEvent()) {
					yield break;
				}

				//GetAttr().transform.rotation = Quaternion.Slerp(GetAttr().transform.rotation, rotation, Time.deltaTime*FastRotateSpeed);
                var newDir = Vector3.Slerp(GetAttr().transform.forward, dir, Time.deltaTime*FastRotateSpeed);
                physic.TurnTo(newDir);
				//GetController().SimpleMove(dir*RunSpeed);
				physic.MoveSpeed(dir*RunSpeed);
				runTime += Time.deltaTime;
				yield return null;	
			}
		}
	}
}

using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class HumanMove : MoveState
	{
		float moveSpeed = 0;
		float walkSpeed = 8.0f;
		float rotateSpeed = 500;
		float speedSmoothing = 10;

		public override void EnterState ()
		{
			Log.AI ("Enter Player Move State");
			base.EnterState ();
			SetAttrState (CharacterState.Running);
			aiCharacter.SetRun ();
		}
        IEnumerator MoveSound(){
            while(!quit){
                var rd = Random.Range(1, 3);
                BackgroundSound.Instance.PlayEffect("heavydirtrun"+rd);
                yield return new WaitForSeconds(0.5f);
            }
        }

		public override IEnumerator RunLogic ()
		{
			var playerMove = GetAttr ().GetComponent<MoveController> ();
			var vcontroller = playerMove.vcontroller;
			var camRight = playerMove.camRight;
			var camForward = playerMove.camForward;

            GetAttr().StartCoroutine(MoveSound());
            var physics = playerMove.GetComponent<PhysicComponent>();
			while (!quit) {
				if (CheckEvent ()) {
					yield break;
				}
				float v = 0;
				float h = 0;
				if (vcontroller != null) {
					h = vcontroller.inputVector.x;//CameraRight 
					v = vcontroller.inputVector.y;//CameraForward
				}
				bool isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;
				isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;
				if(!isMoving) {
					aiCharacter.ChangeState (AIStateEnum.IDLE);
					break;
				}

				Vector3 moveDirection = Vector3.zero;

				Vector3 targetDirection = h * camRight + v * camForward;
				if (targetDirection != Vector3.zero) {
					if (moveSpeed < walkSpeed * 0.3f) {
						moveDirection = Vector3.RotateTowards (moveDirection, targetDirection, rotateSpeed * 2 * Mathf.Deg2Rad * Time.deltaTime, 1000);
						
						moveDirection = moveDirection.normalized;
					}
					// Otherwise smoothly turn towards it
					else {
						moveDirection = Vector3.RotateTowards (moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);			
						moveDirection = moveDirection.normalized;
					}
				}

				var curSmooth = speedSmoothing * Time.deltaTime;
				var targetSpeed = Mathf.Min (targetDirection.magnitude, 1.0f);
				targetSpeed *= walkSpeed;
				moveSpeed = Mathf.Lerp (moveSpeed, targetSpeed, curSmooth);

				var movement = moveDirection * moveSpeed;

                physics.MoveSpeed(movement);
                physics.TurnTo(moveDirection);
				yield return null;
			}
		}
	}
}
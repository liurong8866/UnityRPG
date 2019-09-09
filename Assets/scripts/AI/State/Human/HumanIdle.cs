using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
	public class HumanIdle : IdleState
	{
        bool first= true;
		public override void EnterState ()
		{
			Log.AI ("Enter Idle State");
			base.EnterState ();
			SetAttrState (CharacterState.Idle);
			aiCharacter.SetIdle ();
            if(first){
                first = false;
            }
		}

		public override IEnumerator RunLogic ()
		{
			var playerMove = GetAttr ().GetComponent<MoveController> ();
			var vcontroller = playerMove.vcontroller;

			while (!quit) {
				if(CheckEvent()) {
					yield break;
				}

				float v = 0;
				float h = 0;
				if (vcontroller != null) {
					h = vcontroller.inputVector.x;//CameraRight 
					v = vcontroller.inputVector.y;//CameraForward
				}


				bool isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;
				if(isMoving) {
					aiCharacter.ChangeState(AIStateEnum.MOVE);
				}

				yield return null;
			}
		}
	}
}
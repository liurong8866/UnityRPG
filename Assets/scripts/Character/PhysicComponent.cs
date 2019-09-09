using UnityEngine;
using System.Collections;

namespace MyLib
{
    /// <summary>
    /// 角色所有操作 SimpleMove CharacterController 的代码都要在Physics里面进行 确保同一帧只调用一次
    /// </summary>
	public class PhysicComponent : MonoBehaviour
	{
		float Gravity = 20;

        Vector3 moveValue = Vector3.zero;
		Vector3 motionValue = Vector3.zero;
		//bool skillMoveFade = false;
		public float GetLastSpeed() {
			return lastSpeed;
		}
		float lastSpeed = 0;
		//Vector3 lastPos;
		//技能操控玩家移动
		public void SkillMove(Vector3 pos, float speed) {
			//lastPos = pos;
			var dir = pos - transform.position;
			dir.y = 0;
			Log.Sys("SkillMove Position "+pos+" speed "+speed+" dir "+dir);
			//var speed = dir.magnitude/Time.deltaTime;
			//冲击技能最大的移动速度不超过这些
			//speed = Mathf.Min (20, speed);
			//speed = Mathf.Min (speed, dir.magnitude/Time.deltaTime);
			var newSpeed = Mathf.Lerp (lastSpeed, speed, Time.deltaTime*20);
			Log.Sys ("newSpeed "+lastSpeed+" "+newSpeed);
			lastSpeed = newSpeed;
			motionValue = newSpeed * dir.normalized * Time.deltaTime;
			motionValue.y -= Gravity * Time.deltaTime;
			//GetComponent<CharacterController> ().SimpleMove ();
		}

        public void JumpMove(Vector3 movement) {
            motionValue = movement * Time.deltaTime;
        }
        /// <summary>
        /// 从当前方向旋转到特定方向 
        /// </summary>
        /// <param name="dir">Dir.</param>
        public void TurnTo(Vector3 moveDirection){
            var y1 = transform.eulerAngles.y;
            var y2 = Quaternion.LookRotation (moveDirection).eulerAngles.y;
            var y3 = Mathf.LerpAngle(y1, y2, attribute.GetMoveSpeedCoff());
            transform.rotation = Quaternion.Euler(new Vector3(0, y3, 0));
        }
        public void TurnToImmediately(Vector3 moveDirection) {
            var y2 = Quaternion.LookRotation (moveDirection).eulerAngles.y;
            transform.rotation = Quaternion.Euler(new Vector3(0, y2, 0));
        }
		//键盘操控玩家移动
		public void MoveSpeed(Vector3 moveSpeed) {
			if (!skillMove) {
                moveValue = moveSpeed * attribute.GetMoveSpeedCoff();
				moveValue.y -= Gravity;
			}
		}



		void LateUpdate ()
		{
			//if (!StopMove) {
				//冲击技能结束了但是 速度没有降低为0 继续移动一段时间
				/*
				if(lastSpeed > 0.05) {
					SkillMove(lastPos, 0);
				}
				*/
				if(!skillMove) {
					//GetComponent<CharacterController> ().SimpleMove (moveValue);
					//moveValue = Vector3.zero;
					GetComponent<CharacterController>().Move(moveValue*Time.deltaTime);
					moveValue = new Vector3(0, -Gravity, 0);
                 
				}else {
					GetComponent<CharacterController>().Move(motionValue);
					
				}
			//}
		}


		//冲击技能接管了移动控制
		bool skillMove = false;
		public bool EnterSkillMoveState ()
		{
			if (skillMove == false) {
				skillMove = true;
				lastSpeed = 0;
				moveValue = Vector3.zero;
				motionValue = Vector3.zero;
				return true;
			}
			Log.Critical ("Object In Skill Move Yet "+gameObject.name);
			return false;
		}

		public void ExitSkillMove ()
		{
			skillMove = false;
			moveValue = Vector3.zero;
			motionValue = Vector3.zero;
			lastSpeed = 0;
		}

        NpcAttribute attribute;
        void Start(){
            attribute = GetComponent<NpcAttribute>();
            //StartCoroutine(CheckFalling());
        }

        /*
        IEnumerator CheckFalling(){
            var mtrans = transform;
            while(true){
                if(mtrans.position.y < -50){

                    mtrans.position = new Vector3(mtrans.position.x, 10, mtrans.position.z);
                }
                yield return new WaitForSeconds(1);
            }
        }
        */

	}
}

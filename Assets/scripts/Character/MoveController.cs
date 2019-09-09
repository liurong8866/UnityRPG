using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
	public class VirtualController
	{
		public Vector2 inputVector = Vector2.zero;
	}

    /// <summary>
    ///接受键盘和屏幕虚拟摇杆移动输入
    /// 以及网络命令移动 
    /// </summary>
	public class MoveController : KBEngine.MonoBehaviour
	{
		public VirtualController vcontroller = new VirtualController();
		public Vector3 camRight;
		public Vector3 camForward;

		// Use this for initialization
		void Start ()
		{
			regLocalEvt = new List<MyEvent.EventType> () {
				MyEvent.EventType.MovePlayer,
			};
			RegEvent ();
			Log.Sys("Init Move Player");


			camRight = Camera.main.transform.TransformDirection (Vector3.right);
			camForward = Camera.main.transform.TransformDirection (Vector3.forward);
			camRight.y = 0;
			camForward.y = 0;
			camRight.Normalize ();
			camForward.Normalize ();
		}
		
        /// <summary>
        /// 主镜头旋转了180角度
        /// 输入的左右也要调整 
        /// </summary>
        /// <param name="evt">Evt.</param>
		protected override void OnLocalEvent (MyEvent evt)
		{
			if (evt.type == MyEvent.EventType.MovePlayer) {
				vcontroller.inputVector = evt.vec2;
			}
		}
	}

}
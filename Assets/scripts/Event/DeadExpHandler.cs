using UnityEngine;
using System.Collections;

namespace EventHandler
{
	public class DeadExpHandler : IEventHandler
	{
		#region implemented abstract members of IEventHandler
		public override void Init ()
		{
			regEvent = new System.Collections.Generic.List<MyLib.MyEvent.EventType> () {
				MyLib.MyEvent.EventType.DeadExp,
			};
		}
		public override void OnEvent (MyLib.MyEvent evt)
		{
			Log.Sys ("Dead Exp Add Event");
			var dead = evt as MyLib.DeadExpEvent;
			var player = MyLib.ObjectManager.objectManager.GetMyPlayer ();
			var attr = player.GetComponent<MyLib.NpcAttribute> ();
			attr.ChangeExp (dead.exp);
		}
		#endregion

	}

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EventHandler
{
	public abstract class IEventHandler
	{
		protected List<MyLib.MyEvent.EventType> regEvent;
		public void RegEvent() {
			if (regEvent != null) {
				foreach(MyLib.MyEvent.EventType e in regEvent) {
					MyLib.MyEventSystem.myEventSystem.RegisterEvent(e, OnEvent);
				}
			}
		}
		public abstract void Init();
		public abstract void OnEvent(MyLib.MyEvent evt);
	}
}

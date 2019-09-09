using UnityEngine;
using System.Collections;

namespace MyLib
{
	public class DeadExpEvent : MyEvent
	{
		public int monId;
		public int playerId;
		public int exp;
		public DeadExpEvent() {
			type = EventType.DeadExp;
		}
		
	}

}
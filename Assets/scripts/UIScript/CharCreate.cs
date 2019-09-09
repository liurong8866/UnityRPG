using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MyLib
{
	public class CharCreate : IUserInterface
	{
		List<UIToggle> jobs;

		void Awake ()
		{
			regEvt = new List<MyEvent.EventType> () {
				MyEvent.EventType.UpdateCharacterCreateUI,
				MyEvent.EventType.CreateSuccess,
			};
			RegEvent ();
			SetCallback ("createChar", OnCreate);

			jobs = new List<UIToggle> ();
			for (var i = 1; i <= 3; i++) {
				var tog = GetName ("job" + i).GetComponent<UIToggle> ();
				jobs.Add (tog);
				var temp = i;
				UIEventListener.Get (tog.gameObject).onSelect = (go, ret) => {
					if (ret) {
						selJob = temp;
						UpdateFrame();
					}
				};
			}
		}

		private int selJob = 1;

		void OnCreate (GameObject g)
		{
			var name = GetInput ("NameInput").value;
			Log.GUI ("Create Char is " + name + " ");
			if (string.IsNullOrEmpty (name) || name.Length > 20) {
				WindowMng.windowMng.ShowNotifyLog ("名字不能为空！且名字长度不能大于20!", 2);
				return;
			}
			CharSelectProgress.charSelectLogic.CreateChar (name, selJob);
		}

		protected override void OnEvent (MyEvent evt)
		{
			if (evt.type == MyEvent.EventType.UpdateCharacterCreateUI) {
				UpdateFrame ();
			} else if (evt.type == MyEvent.EventType.CreateSuccess) {
				WindowMng.windowMng.ReplaceView ("UI/CharSelect2", false);
				MyEventSystem.myEventSystem.PushEvent (MyEvent.EventType.UpdateSelectChar);
			}
		}

		void UpdateFrame ()
		{
			FakeObjSystem.fakeObjSystem.OnUIShown (-1, null, selJob);
		}
	}
}

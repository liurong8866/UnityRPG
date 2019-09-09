
/*
Author: liyonghelpme
Email: 233242872@qq.com
*/

/*
Author: liyonghelpme
Email: 233242872@qq.com
*/
using UnityEngine;
using System.Collections;

/*
 * Player Touch Sphere Enter Into Battle Scene 
 */ 
namespace MyLib {
	public class EntranceTrigger : KBEngine.MonoBehaviour {
		public int NextSceneId = -1;
		void Awake() {
			regEvt = new System.Collections.Generic.List<MyEvent.EventType> () {
				MyEvent.EventType.OkEnter,
			};
			RegEvent ();
		}
		protected override void OnEvent (MyEvent evt)
		{
			if (evt.type == MyEvent.EventType.OkEnter) {
				OnOk(null);
			}
		}
		//bool enterYet = false;
		
		void OnOk(GameObject g) {	
			Log.GUI ("OnOk Next Scene "+NextSceneId);
			WorldManager.worldManager.StartCoroutine(WorldManager.worldManager.ChangeScene (NextSceneId, false));
		}


		void OnTriggerEnter(Collider other) {
            /*
			if (other.tag == "Player" && NextSceneId != -1 && other.gameObject == ObjectManager.objectManager.GetMyPlayer()) {
				Debug.Log("Enter Level");
				var tips = WindowMng.windowMng.PushView("UI/tips").GetComponent<TipsPanel>();
				tips.SetOk(OnOk);
			}
            */
		}

		void OnTriggerExit(Collider other) {
			if (other.tag == "Player") {
			}
		}
	}

}
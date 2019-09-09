using UnityEngine;
using System.Collections;

namespace MyLib {
	public class PopupTextManager : MonoBehaviour {
		static PopupTextManager _pop = null;
		public static PopupTextManager popTextManager{
			get {
				if(_pop == null) {
					var g = new GameObject();
					_pop = g.AddComponent<PopupTextManager>();
				}
				return _pop;
			}
		}
		GameObject hudText = null;
		GameObject popUpPanel = null;
		GameObject purpleText = null;

		//TODO: Pool缓冲池处理
		public void ShowText(object text, Transform target) {
			if (popUpPanel == null) {
				var temp = Resources.Load<GameObject> ("UI/popUpDamage");
				popUpPanel = NGUITools.AddChild(WindowMng.windowMng.GetUIRoot(), temp);
				hudText = Resources.Load<GameObject>("UI/MyHudLabel");
			}

			var label = NGUITools.AddChild (popUpPanel, hudText);
			label.GetComponent<FollowTarget> ().target = target.gameObject;
			//label.SetActive (true);

			label.GetComponent<HUDText> ().Add (text, Color.white, 2f);
			StartCoroutine (WaitRemove(label));
		}

		public void ShowRedText(object text, Transform target) {
			Log.GUI ("Show Red Text Here "+text);
			if (popUpPanel == null) {
				var temp = Resources.Load<GameObject> ("UI/popUpDamage");
				popUpPanel = NGUITools.AddChild(WindowMng.windowMng.GetUIRoot(), temp);
			}
			if(hudText == null) {
				hudText = Resources.Load<GameObject>("UI/MyHudLabel");
			}
			
			var label = NGUITools.AddChild (popUpPanel, hudText);
			label.GetComponent<FollowTarget> ().target = target.gameObject;
			//label.SetActive (true);
			
			label.GetComponent<HUDText> ().Add (text, new Color(0.8f, 0.1f, 0.1f), 0.1f);
			StartCoroutine (WaitRemove(label));
		}

		public void ShowPurpleText (object text, Transform target)
		{
			if (popUpPanel == null) {
				var temp = Resources.Load<GameObject> ("UI/popUpDamage");
				popUpPanel = NGUITools.AddChild(WindowMng.windowMng.GetUIRoot(), temp);
			}
			if(purpleText == null) {
				purpleText = Resources.Load<GameObject>("UI/MyHudLabelPurple");
			}
			
			var label = NGUITools.AddChild (popUpPanel, purpleText);
			label.GetComponent<FollowTarget> ().target = target.gameObject;
			//label.SetActive (true);
			
			label.GetComponent<HUDText> ().Add (text, new Color(223/255.0f, 31/255.0f, 246/255.0f), 0.1f);
			StartCoroutine (WaitRemove(label));
		}

		IEnumerator WaitRemove(GameObject label) {
			var text = label.GetComponent<HUDText> ();
			while (true) {
				if(!text.isVisible) {
					break;
				}
				yield return null;
			}
			GameObject.Destroy (label);
		}

		void OnDestroy() {
			_pop = null;
		}
	}

}
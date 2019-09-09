
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

namespace MyLib
{
	public class CutSceneTrigger : MonoBehaviour
	{
		public float Radius = 3;
		[ButtonCallFunc()]
		public bool SetRadius;
		public void SetRadiusMethod ()
		{
			GetComponent<SphereCollider> ().radius = Radius;
		}

		public float TriggerRange;

		GameObject uiRoot;

		void Awake ()
		{
			uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
			GetComponent<Collider>().isTrigger = true;
		}
		// Use this for initialization
		void Start ()
		{
	
		}
		//Logic Connect 
		void StartCutScene ()
		{
			/*
			var menuUI = Resources.Load ("UI/cinematicmenu") as GameObject;
			var cm = Instantiate (menuUI) as GameObject;
			cm.name = "cinematicmenu";
			cm.transform.parent = uiRoot.transform;
			cm.transform.localPosition = Vector3.zero;
			cm.transform.localRotation = Quaternion.identity;
			cm.transform.localScale = Vector3.one;
			*/
			var menuUI = Util.FindChildRecursive(uiRoot.transform, "cinematicmenu").gameObject;
			menuUI.SetActive (true);


			var norui = Util.FindChildRecursive (uiRoot.transform, "NormalUI").gameObject;
			//norui.GetComponent<UIPanel>()
			norui.SetActive (false);

			var ren = Util.FindChildRecursive (transform.parent, "DialogRender");
			ren.GetComponent<DialogRender> ().StartCutScene ();

		}

		bool TriggerYet = false;

		void FixedUpdate ()
		{
			if (!TriggerYet) {
				Collider[] cols = Physics.OverlapSphere (transform.position, Radius);
				foreach (Collider c in cols) {
					if (c.tag == "Player") {
						TriggerYet = true;
						StartCutScene ();
						break;
					}
				}
			}
		}

		// Update is called once per frame
		void Update ()
		{
	
		}
	}

}

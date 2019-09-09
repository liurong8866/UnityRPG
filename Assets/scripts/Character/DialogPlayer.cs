using UnityEngine;
using System.Collections;

namespace MyLib
{
	[RequireComponent(typeof(ShadowComponent))]
	[RequireComponent(typeof(NpcEquipment))]
	[RequireComponent(typeof(MyAnimationEvent))]
	[RequireComponent(typeof(KBEngine.KBNetworkView))]
	public class DialogPlayer : MonoBehaviour
	{
		void Awake() {
			//GetComponent<ShadowComponent> ().CreateShadowPlane ();
		}

		//战斗姿态更适合展示武器 
		void SetIdle() {
			//var peace = LevelInit.IsPeaceLevel ();
			//var peace = WorldManager.worldManager.IsPeaceLevel();
			var idleName = "stand";

			GetComponent<Animation>().CrossFade (idleName);
			GetComponent<Animation>() [idleName].wrapMode = WrapMode.Loop;
		}
		void OnEnable() {
			SetIdle ();
		}
		// Use this for initialization
		void Start ()
		{
			//SetIdle ();
		}

		// Update is called once per frame
		void Update ()
		{
	
		}
	}
}

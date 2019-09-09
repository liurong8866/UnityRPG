
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

namespace MyLib {
	/*
	[RequireComponent(typeof(NpcAttribute))]
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(ShadowComponent))]
	*/
	public class StoryNpcAI : AIBase {

		public float WalkSpeed = 2f;
		public float RunSpeed = 5;
		public float FastRotateSpeed = 10;

		MyAnimationEvent myAnimationEvent;
		//CharacterController controller;
		
		void Awake() {
			attribute = GetComponent<NpcAttribute> ();
			myAnimationEvent = GetComponent<MyAnimationEvent>();
			//controller = GetComponent<CharacterController>();
			GetComponent<Animation>().Play("idle");
			GetComponent<Animation>() ["idle"].wrapMode = WrapMode.Loop;
			//GetComponent<ShadowComponent> ().CreateShadowPlane ();
		}
			

		// Use this for initialization
		void Start () {
			StartCoroutine (WaitForHit());
		}

		IEnumerator WaitForHit() {
			while (true) {
				if(myAnimationEvent.onHit) {
					break;
				}
				yield return null;
			}
			attribute.IsDead = true;
			GetComponent<Animation>().CrossFade("die");
			GetComponent<Animation>() ["die"].speed = 2;
			GetComponent<Animation>() ["die"].wrapMode = WrapMode.Once;
		}
		
		// Update is called once per frame
		void Update () {
		
		}


	}

}
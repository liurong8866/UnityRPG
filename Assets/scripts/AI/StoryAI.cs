
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
 * CutScene Control Player To Move And Kill Somebody 
 * AI Script
 */ 
namespace MyLib {
	[RequireComponent(typeof(CommonAI))]
	public class StoryAI : MonoBehaviour {
		MyAnimationEvent animationEvent;
		CommonAI commonAI;

		void Awake() {
			commonAI = GetComponent<CommonAI> ();
			animationEvent = GetComponent<MyAnimationEvent> ();
		}
		// Use this for initialization
		void Start () {
			StartCoroutine (WaitStory ());
		}

		IEnumerator KillNpc(MyAnimationEvent.Message msg) {
			GetComponent<PlayerAIController> ().enabled = false;

			NpcAttribute attribute = GetComponent<NpcAttribute> ();
			attribute._characterState = CharacterState.Story;


			DialogRender.KillNpcMessage m = (DialogRender.KillNpcMessage)msg;
			var targetName = m.dialog.dialog;
			var target = GameObject.Find (targetName);

			Debug.Log("start kill Npc");	
			yield return StartCoroutine (commonAI.MoveToPos (target.transform.position));
			//Rotation And Move To Target Position 

			yield return StartCoroutine (commonAI.KillNpc (target));

			attribute._characterState = CharacterState.Idle;
			GetComponent<Animation>().CrossFade("idle");

			GetComponent<PlayerAIController> ().enabled = false;
		}

		IEnumerator WaitStory() {
			while (true) {
				var msg = animationEvent.CheckMsg(MyAnimationEvent.MsgType.KillNpc);
				if(msg != null) {
					StartCoroutine (KillNpc (msg));
					Debug.Log("Store AI handle");
				}
				yield return null;
			}
		}

		// Update is called once per frame
		void Update () {
		
		}
	}

}

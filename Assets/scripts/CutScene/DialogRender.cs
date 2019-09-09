
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
using System.Collections.Generic;

/*
 * Connect DialogCutScene And uiRoot 
 * dirty Code is here
 */
namespace MyLib {
	public class DialogRender : MonoBehaviour {

		GameObject DialogCutScene;
		GameObject uiRoot;
		List<Dialog> dialogs;
		UILabel otherDialog;
		UILabel myDialog;

		GameObject player;

		void Awake() {
			DialogCutScene = GameObject.Find("DialogCutScene");
			uiRoot = GameObject.Find("UI Root");
			dialogs = DialogCutScene.GetComponent<DialogCutScene> ().Dialogs;

			player = GameObject.FindGameObjectWithTag("Player");
		}

		// Use this for initialization
		void Start () {


			//var ani = DialogCutScene.GetComponent<DialogCutScene>().track;
			//animation.AddClip(ani, "master");
			//animation.Play ("master");
		}

		public void StartCutScene() {
			var cinema = Util.FindChildRecursive(uiRoot.transform, "cinematicmenu");
			otherDialog = Util.FindChildRecursive(cinema, "OtherDialog").GetComponent<UILabel>();
			myDialog = Util.FindChildRecursive(cinema, "MyDialog").GetComponent<UILabel>();
			
			otherDialog.text = "";
			myDialog.text = "";

			var ani = DialogCutScene.GetComponent<DialogCutScene> ().track;
			//gameObject.AddComponent<Animation> ();
			GetComponent<Animation>().AddClip(ani, "master");
			GetComponent<Animation>().Play("master");

			/*
			 * Stop Player Control Move
			 */
			player.GetComponent<PlayerAIController> ().enabled = false;
			player.GetComponent<Animation>().CrossFade("idle");

		}

		void PlayDialog(int clipId) {
			var dia = dialogs[clipId];
			if (dia.who == 0) {
				otherDialog.text = dia.dialog;
			} else {
				myDialog.text = dia.dialog;
			}
		}

		public class KillNpcMessage : MyAnimationEvent.Message {
			public Dialog dialog;
			public KillNpcMessage(Dialog d) {
				dialog = d;
			}
		}

		IEnumerator WaitForNpcDead(string npcName) {
			var npc = GameObject.Find (npcName);
			var attribute = npc.GetComponent<NpcAttribute> ();
			while (!attribute.IsDead) {
				yield return null;
			}

			yield return new WaitForSeconds (1);
			RealFinish ();
		}
		void KillNpc(int clipId) {
			var cinema = Util.FindChildRecursive(uiRoot.transform, "cinematicmenu");
			var cp = Util.FindChildRecursive(cinema, "ContentPanel").gameObject;
			cp.SetActive (false);

			player.GetComponent<MyAnimationEvent>().InsertMsg(new KillNpcMessage(dialogs[clipId]));

			StartCoroutine (WaitForNpcDead(dialogs[clipId].dialog));
		}

		void RealFinish() {
			var menu = Util.FindChildRecursive(uiRoot.transform, "cinematicmenu").gameObject;
			menu.SetActive (false);
			var normalUI = Util.FindChildRecursive (uiRoot.transform, "NormalUI").gameObject;
			normalUI.SetActive (true);
			
			var player = GameObject.FindGameObjectWithTag("Player");
			player.GetComponent<StoryAI> ().enabled = false;
			player.GetComponent<PlayerAIController> ().enabled = true;

			var battleManager = GameObject.Find("BattleManager");
			battleManager.GetComponent<BattleManager> ().waveNum = 2;

		}

		void Finish(int clipId) {


		}

		// Update is called once per frame
		void Update () {
		
		}
	}

}
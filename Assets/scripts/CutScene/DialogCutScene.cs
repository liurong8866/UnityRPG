
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

namespace MyLib {
	public enum DialogType {
		Dialog,
		Action,
		Finish,
	}

	[System.Serializable]
	public class Dialog {
		public DialogType type = DialogType.Dialog;
		public string functionName;
		public string dialog;
		public float startTime = 0;
		public float endTime = 3;
		public int who = 0;

		public float duration {
			get {
				return endTime-startTime;
			}
		}
	}

	public class DialogCutScene : MonoBehaviour {
		public delegate void StartDialog(Dialog d);

		public StartDialog startDialog;



		public float GetStartTime() {
			if (Dialogs.Count == 0)
				return 0;
			return Dialogs [Dialogs.Count - 1].endTime;
		}
		public int GetWho() {
			if (Dialogs.Count == 0)
				return 0;
			return 1-Dialogs [Dialogs.Count - 1].who;
		}

		[ButtonCallFunc()]
		public bool AddDialog;
		public void AddDialogMethod() {
			var d = new Dialog ();
			d.startTime = GetStartTime ();
			d.endTime = d.startTime + defaultDuration;
			d.who = GetWho ();
			Dialogs.Add (d);
		}

		[ButtonCallFunc()]
		public bool AddAction;
		public void AddActionMethod() {
			var d = new Dialog ();
			d.type = DialogType.Action;
			d.startTime = GetStartTime ();
			d.endTime = d.startTime + defaultDuration;
			Dialogs.Add (d);
		}

		[ButtonCallFunc()]
		public bool AddFinish;
		public void AddFinishMethod() {
			var d = new Dialog ();
			d.type = DialogType.Finish;
			d.startTime = GetStartTime ();
			d.endTime = d.startTime + defaultDuration;
			Dialogs.Add (d);
		}

		[ButtonCallFunc()]
		public bool SetDuration;
		public void SetDurationMethod() {
			int c = 0;
			foreach (Dialog d in Dialogs) {
				d.startTime = c*defaultDuration;
				c++;
			}
		}

		public float defaultDuration = 3;

		public List<Dialog> Dialogs;
		public Dialog currentDialog;


		public AnimationClip track {
			get {
				AnimationClip _track = new AnimationClip();
				int cId = 0;
				foreach(Dialog clip in Dialogs) {
					AnimationEvent start = new AnimationEvent();
					start.time = clip.startTime;
					start.intParameter = cId;
					if(clip.type == DialogType.Dialog) {
						start.functionName = "PlayDialog";
						//start.objectReferenceParameter = this;
					}else if(clip.type == DialogType.Finish) {
						start.functionName = "Finish";
					}else {
						start.functionName = clip.functionName;
					}
					_track.AddEvent(start);
					cId++;
				}
				return _track;
			}
		}

		public void PlayDialog(Dialog clip) {
			
		}

		void PlaySubtitle(int clipId) {
			if (startDialog != null) {
				startDialog(Dialogs[clipId]);
			}
			var clip = Dialogs [clipId];
			currentDialog = clip;
			StartCoroutine (StopSubtitle(clip.duration));
		}

		IEnumerator StopSubtitle(float duration) {
			yield return new WaitForSeconds (duration);
			currentDialog = null;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}

}

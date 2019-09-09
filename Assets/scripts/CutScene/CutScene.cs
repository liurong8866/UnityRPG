
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
//using UnityEditor;

public class CutScene : MonoBehaviour {
	public enum MediaType {
		Shots = 0,
		Actors = 1,
		Audio = 2,
		Subtitles = 3,
	}
	public AnimationClip masterClip;

	[ButtonCallFunc()]
	public bool AddTrackAtt;
	public void AddTrackAttMethod() {
		AddTrack (MediaType.Subtitles);
	}


	[CutSceneAttribute()]
	public bool AddSubtitle;

	public void AddSubtitleMethod(string dialog){
		NewSubtitle (dialog);
	}

	public CutSceneTrack [] tracks {
		get {
			return GetComponentsInChildren<CutSceneTrack>();
		}
	}



	public CutSceneTrack AddTrack(MediaType mt) {
		int id = 0;
		foreach (CutSceneTrack t in tracks) {
			if(id == t.id) {
				id++;
			}else {
				break;
			}
		}

		CutSceneTrack track = gameObject.AddComponent<CutSceneTrack> ();
		track.id = id;
		track.type = mt;
		track.name = "Subtitle";
		return track;
	}


	public CutSceneSubtitles NewSubtitle(string dialog) {
		GameObject sub = transform.Find ("Subtitles").gameObject;
		CutSceneSubtitles subtitle = sub.AddComponent<CutSceneSubtitles> ();
		subtitle.dialog = dialog;
		subtitle.cutScene = this;
		return subtitle;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

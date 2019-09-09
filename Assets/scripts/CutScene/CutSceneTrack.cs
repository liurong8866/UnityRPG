
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

public class CutSceneTrack : MonoBehaviour {
	public CutScene.MediaType type = CutScene.MediaType.Subtitles;
	public List<CutSceneClip> clips = new List<CutSceneClip>();

	//CutSceneSubtitles currentSubtitle;
	[HideInInspector]
	public int id = 0;


	public AnimationClip track {
		get {
			AnimationClip _track = new AnimationClip();
			foreach(CutSceneClip clip in clips) {
				AnimationEvent start = new AnimationEvent();
				start.time = clip.timelineStart;
				start.functionName = clip.startFunction;
				start.objectReferenceParameter = clip;
				_track.AddEvent(start);
			}
			return _track;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void PlaySubtitle(CutSceneClip clip) {
		//currentSubtitle = (CutSceneSubtitles)clip.master;
		StartCoroutine (StopSubtitle(clip.duration));
	}
	IEnumerator StopSubtitle(float duration) {
		yield return new WaitForSeconds (duration);
		//currentSubtitle = null;
	}
}

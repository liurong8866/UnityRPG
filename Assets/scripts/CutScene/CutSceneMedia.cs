
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

public class CutSceneMedia : MonoBehaviour {
	[ButtonCallFunc()]
	public bool AddClip;
	[HideInInspector]
	public CutScene cutScene;
	public void AddClipMethod() {
		if (cutScene.tracks.Length == 0) {
			cutScene.AddTrack(type);
		}
		CutSceneClip newClip = (CutSceneClip)ScriptableObject.CreateInstance (typeof( CutSceneClip));
		//new CutSceneClip (this);
		newClip.master = this;
		cutScene.tracks [0].clips.Add (newClip);

	}

	public CutScene.MediaType type {
		get {
			if(this.GetType() == typeof(CutSceneSubtitles)) {
				return CutScene.MediaType.Subtitles;
			}
			return CutScene.MediaType.Subtitles;
		}
	}

}

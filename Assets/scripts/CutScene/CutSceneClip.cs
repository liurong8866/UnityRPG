
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
using System;

public class CutSceneClip : ScriptableObject  {
	public CutSceneMedia master;
	public float timelineStart = 0f;
	public float inPoint = 0f;
	public float outPoint = 5f;

	public string startFunction {
		get {
			if(master is CutSceneSubtitles) {
				return "PlaySubtitle";
			}else {
				return "UnknownFunction";
			}
		}
	}

	public float duration {
		get {
			return outPoint - inPoint;
		}
	}
	/*
	public CutSceneClip(CutSceneMedia master) {
		this.master = master;
		if (master is CutSceneSubtitles) {
			name = ((CutSceneSubtitles)master).dialog;
		} else {
			name = "";
		}

	}
	*/
}

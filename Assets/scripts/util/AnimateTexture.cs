
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

public class AnimateTexture : MonoBehaviour {
    public string TextureName = "sharedtextures/caustics/Caustics_WATER_{0}";
	public int TextureNum = 80;
	public float Duration = 0.8f;
    public int interval = 10;

	float passTime = 0;
	float delta;
	void Awake() {
		var tname = string.Format(TextureName, 0);
		GetComponent<Renderer>().material.SetTexture ("_AnimTex", Resources.Load<Texture>(tname));
	}   
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        delta = Duration / TextureNum;
		passTime += Time.deltaTime;
		int fn = Mathf.FloorToInt(passTime / delta);
		fn = fn % TextureNum;
        fn /= interval;
        fn *= interval;
		var tname = string.Format (TextureName, fn);
		GetComponent<Renderer>().material.SetTexture ("_AnimTex", Resources.Load<Texture> (tname));
	}
}

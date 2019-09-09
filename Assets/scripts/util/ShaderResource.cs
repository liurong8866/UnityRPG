
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
 * Need To dynamic load Shader And Texture
 */ 
public class ShaderResource : MonoBehaviour {
	//public static Shader burnShader;
	public Texture cloudImg;
	public GameObject bloodHit;
	public GameObject deathBlood;
	public GameObject swordHit;

	public static ShaderResource shaderResource;
	public List<Shader> linkShader;
	void Awake() {
		shaderResource = this;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

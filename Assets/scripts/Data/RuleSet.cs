
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


public class RuleSet : MonoBehaviour {
	public string Name = "mine";
	public float TileBasis = 4;
	public bool RequiresExit = true;
	public float ChunkWidthBasis = 25;
	public float ChunkHeightBasis = 25;

	public AudioClip Music;
	public Texture TorchLight;
	public Texture ShadowFade;
	public Texture LightMask;
	public Texture RimLight;
	public Texture Water;

	public float LightMapRed = 42;
	public float LightMapGreen = 48;
	public float LightMapBlue = 39;
	public float AmbientRed = 152;
	public float AmbientGreen = 152;
	public float AmbientBlue = 152;
	public float DirectionalRed = 70;
	public float DirectionalGreen = 70;
	public float DirectionalBlue = 70;

	public float DirectionalX = 40;
	public float DirectionalY = 150;
	public float DirectionalZ = -40;
	public float DirectionalIntensity = 1.9f;

	public int FogRed = 0;
	public int FogGreen = 0;
	public int FogBlue = 0;
	public int FogStart = 24;
	public int FogEnd = 45;

	public GameObject LevelParticle;
	public int MinChunks = 1;
	public int MaxChunks = 2;

	//Monster stay together
	public bool Randomized = true;
	public bool Populated = true;

	//1X1ENTRANCE_W_PB_A
	//1X1ENTRANCE_N_PB_A
	//1X1ENTRANCE_S 
	//1X1NE not ENTRANCE_CHUNK
	//
	[System.Serializable]
	public class ChunkType {
		public string Name = "1X1ENTRANCE_E";
		public bool EntranceChunk = true;
		public int Width = 1;
		public int Height = 1;
		//Multiple Exit of Map
		public Vector3 Exit1 = new Vector3(-48, 0, 48);
		public Vector3 Exit2;

		public List<string> InclusiveFiles;
	}

	public List<ChunkType> ChunkTypes;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

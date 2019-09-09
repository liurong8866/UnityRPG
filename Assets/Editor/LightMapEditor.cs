using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/*
 * 为所有子物体中 shader 为 lightmapFloor的 配置对应的材质 属性
 */
namespace MyLib {
	[CustomEditor(typeof(SetLightMap))]
	public class LightMapEditor : Editor {
		Texture rt;
		TraceMainCamera tm;

		void OnEnable() {
			rt = Resources.Load<Texture>("Textures/lightMap");
			//tm = GameObject.FindObjectOfType<TraceMainCamera>();
			tm = Resources.Load<GameObject> ("levelPublic/lightMapCamera").GetComponent<TraceMainCamera> ();
		}
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		void AdjustAllChildren(GameObject g, List<Material> lmMat) {
			string[] allShaders = new string[]{
				"Custom/lightMapFloor",
				"Custom/lightMapFloorAlpha",
				"Custom/vertexColorTerrian",
				"Custom/vertexColorTerrianAlpha",
				"Custom/vertexColourIllumShader",
				"Custom/vertexColorAlphaRejectBlend",
			};


			List<Shader> sh = new List<Shader> ();
			foreach (string s in allShaders) {
				sh.Add(Shader.Find(s));
			}

			if (g.GetComponent<Renderer>() != null) {
				for(int i = 0; i < g.GetComponent<Renderer>().sharedMaterials.Length; i++) {
					Debug.Log("LightMapEditor::AdjustAllChildren set up lightMap "+g);
					if(sh.Contains(g.GetComponent<Renderer>().sharedMaterials[i].shader)) {
					//if(g.renderer.sharedMaterials[i].shader == lm) {
						g.GetComponent<Renderer>().sharedMaterials[i].SetTexture("_LightMap", rt);
						lmMat.Add(g.GetComponent<Renderer>().sharedMaterials[i]);
						if(tm != null && !tm.Materials.Contains(g.GetComponent<Renderer>().sharedMaterials[i])) {
							tm.Materials.Add(g.GetComponent<Renderer>().sharedMaterials[i]);
						}
					}
				}
			}

			foreach (Transform t in g.transform) {
				AdjustAllChildren(t.gameObject, lmMat);
			}
		}
		public override void OnInspectorGUI() {
			if(GUILayout.Button("SetUp LightMap"))
			{
				SetLightMap s = (SetLightMap)target;
				List<Material> lmMat = new List<Material>();
				AdjustAllChildren(s.gameObject, lmMat);

				Debug.Log("add material "+lmMat.Count);
				foreach(Material mat in lmMat) {
					Debug.Log("mat name "+mat);
				} 

				//this Scene LightMap Camera Apply LightMapCamera back
				//PrefabUtility.DisconnectPrefabInstance(tm.gameObject);
			}
		}
	}

}

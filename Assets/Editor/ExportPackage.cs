using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ExportPackage : MonoBehaviour {
    [MenuItem("MyMenu/ExportPackage")]
    static void Export(){
        var tar = Path.Combine(Application.dataPath.Replace("Assets", ""), "tar.unitypackage");
        var assets = new List<string>();
        var g = Selection.activeGameObject;
        if(g != null) {
            var roots = new GameObject[]{g};
            var objs = EditorUtility.CollectDependencies(roots);
            var realObjs = new List<Object>();
            tar = g.name+".unitypackage";

            foreach(var o in objs){
                //var ot = o.GetType();
                /*
                if(ot == typeof(GameObject)
                   || ot == typeof(Material)
                   || ot == typeof(Texture2D)
                   || ot == typeof(MeshRenderer)
                   || ot == typeof(MeshRenderer)
                   || ot == typeof(SkinnedMeshRenderer)
                   ){
                    if(ot == typeof(GameObject)){
                        
                    }else {


                    }

                }
                */
                if(o.GetType().IsSubclassOf(typeof(MonoBehaviour)) || o.GetType() == typeof(MonoBehaviour)){
                }else if(o.GetType() == typeof(MonoScript)){
                }else if(o.GetType() == typeof(Transform)){
                }else if(o.GetType() == typeof(GameObject)){
                }
                else {
                    var path = AssetDatabase.GetAssetPath(o);
                    if(string.IsNullOrEmpty(path)) {
                    }else {
                        assets.Add(path);
                        realObjs.Add(o);
                        Debug.Log(path);
                    }                    
                }
                /*
                if(o.GetType().IsSubclassOf(typeof(MonoBehaviour)) || o.GetType() == typeof(MonoBehaviour)){
                }else if(o.GetType() == typeof(MonoScript)){
                }else if(o.GetType() == typeof(Transform)){
                }else if(o.GetType() == typeof(MeshFilter)){
                }else if(o.GetType() == typeof(MeshRenderer)){
                }
                else {

                }
                */
            }
            realObjs.Add(g);
            Selection.objects = realObjs.ToArray();

            AssetDatabase.ExportPackage(assets.ToArray(), tar);
            Debug.Log("Save As "+tar);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

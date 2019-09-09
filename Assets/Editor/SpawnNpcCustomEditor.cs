using UnityEngine;
using System.Collections;
using MyLib;
using UnityEditor;

[CustomEditor(typeof(SpawnNpc))]
public class SpawnNpcCustomEditor : Editor {
    void OnEnable() {
        if(Application.isEditor && !EditorApplication.isPlaying) {
            var st = target as SpawnNpc ;
            EditorApplication.update = st.UpdateEditor;
        }
    }
    void OnDisable() {
        EditorApplication.update = null;
    }
	
}

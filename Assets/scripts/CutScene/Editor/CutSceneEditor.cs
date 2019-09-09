
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
using UnityEditor;
using System.Reflection;


//[CustomEditor(typeof(CutSceneClip))]
public class CutSceneEditor : Editor {

	[MenuItem("GameObject/Create Other/Cutscene")]
	static void CreateCutScene() {
		GameObject newObj = new GameObject ("CutScene", typeof(CutScene));
		CutScene newScene = newObj.GetComponent<CutScene> ();
		newScene.AddTrack (CutScene.MediaType.Subtitles);
		GameObject subtitles = new GameObject("Subtitles");
		subtitles.transform.parent = newObj.transform;
		
		AnimationClip masterClip = new AnimationClip ();
		newScene.masterClip = new AnimationClip ();
		newScene.gameObject.AddComponent<Animation> ();
		newScene.GetComponent<Animation>().AddClip(masterClip, "master");
		
		newScene.GetComponent<Animation>().playAutomatically = false;
		newScene.GetComponent<Animation>().wrapMode = WrapMode.Once;
		
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}


[CustomPropertyDrawer(typeof(CutSceneAttribute))]
public class CutSceneAttributeDrawer : PropertyDrawer {
	const int butHeight = 16;
	
	CutSceneAttribute cutsceneAttribute {
		get {
			return (CutSceneAttribute) attribute;
		}
	}
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
		return base.GetPropertyHeight (prop, label) + butHeight;
	}
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
		Rect textFieldPosition = position;
		textFieldPosition.height = butHeight;
		cutsceneAttribute.dialog = EditorGUI.TextField (textFieldPosition, new GUIContent(label), cutsceneAttribute.dialog);
		
		Rect butPosition = position;
		butPosition.y += butHeight;
		butPosition.height = butHeight;
		
		if (GUI.Button (butPosition, prop.propertyPath)) {
			var cs =  prop.serializedObject.targetObject;
			MethodInfo mi = cs.GetType().GetMethod(prop.propertyPath+"Method");
			object[] paramsArr = new object[]{cutsceneAttribute.dialog};
			mi.Invoke(cs, paramsArr);
		}
	}
}

[CustomPropertyDrawer(typeof(ButtonCallFunc))] 
public class ButtonCallFuncDrawer : PropertyDrawer{
	const int butHeight = 16;
	const int pad = 5;
	
	ButtonCallFunc cutsceneAttribute {
		get {
			return (ButtonCallFunc) attribute;
		}
	}
	public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
		return base.GetPropertyHeight (prop, label)+pad;
	}
	public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
		//Rect textFieldPosition = position;
		//textFieldPosition.height = butHeight;
		//cutsceneAttribute.dialog = EditorGUI.TextField (textFieldPosition, new GUIContent(label), cutsceneAttribute.dialog);
		
		Rect butPosition = position;
		//butPosition.y += butHeight;
		//butPosition.height = butHeight;
		
		if (GUI.Button (butPosition, prop.propertyPath)) {
			var cs =  prop.serializedObject.targetObject;
			MethodInfo mi = cs.GetType().GetMethod(prop.propertyPath+"Method");
			//object[] paramsArr = new object[]{cutsceneAttribute.dialog};
			mi.Invoke(cs, null);
		}
	}
}
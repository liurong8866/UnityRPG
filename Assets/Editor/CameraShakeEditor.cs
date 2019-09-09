using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MyLib.CameraShakeData))]
public class CameraShakeEditor : Editor {
	SerializedProperty shakeCurve;
	static float LabelWidth = 130f;
	void DrawCurve01(string label, string tooltip, SerializedProperty obj) {
		GUIContent content = null;
		
		if (string.IsNullOrEmpty(tooltip))
			content = new GUIContent(label);
		else
			content = new GUIContent(label,tooltip);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(content,GUILayout.Width(LabelWidth));
		obj.animationCurveValue = EditorGUILayout.CurveField(obj.animationCurveValue,Color.green, new Rect(0f,-2f,1f,4f));
		EditorGUILayout.EndHorizontal();
	}

	void DrawShakeCurve() {

		DrawCurve01 ("Camera", "", shakeCurve);

	}
	static void LoadStyle() {
		
	}
	void InitializeProp() {
		shakeCurve = serializedObject.FindProperty ("shakeCurve");
	}
	void OnEnable() {
		LoadStyle ();
		InitializeProp ();
	}


	public override void OnInspectorGUI ()
	{
		serializedObject.Update ();
		DrawShakeCurve ();
		serializedObject.ApplyModifiedProperties ();
	}
}

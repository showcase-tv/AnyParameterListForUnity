using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using APL;

[CustomEditor(typeof(APL.AnyParameter))]
public class AnyParameterEditor : Editor {
	public enum Action {
		MoveUp,
		MoveDown,
		Delete,
	};

	private AnyParameter param;
	private bool foldout = true;
	private GUIStyle style;
	public delegate void OnParameterAction(AnyParameter param, Action action);
	public OnParameterAction OnParameterEvent;

	void OnEnable() {
		style = new GUIStyle (EditorStyles.foldout);
		style.fontStyle = FontStyle.Bold;
		param = (AnyParameter)target;
	}

	public void OnInspectorGUIWithParent(AnyParameterListEditor parent) {
		OnInspectorGUI ();
	}

	public override void OnInspectorGUI()
	{
		
		GUILayout.BeginHorizontal ();
		foldout = EditorGUILayout.Foldout(foldout, param.Title, true, style);
		DrawActionButton ();
		GUILayout.EndHorizontal ();
		if (foldout) {
			// start modifying
			serializedObject.Update();

			////// for debug
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("parent"));

			// Type GUI
			DrawTypeNameField ();

			// Id GUI
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("id"));

			// Value GUI
			if (param.majorType != null) {
				DrawValueField ();
			}

			// comment GUI
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("comment"));
			// apply to object
			serializedObject.ApplyModifiedProperties();
		}
	}

	void DrawTypeNameField() {
		var typeNameProperty = serializedObject.FindProperty ("typeName");
		var typeKeys = AnyParameter.TypeKeys;
		int selected = System.Array.IndexOf(typeKeys, typeNameProperty.stringValue);
		selected = EditorGUILayout.Popup("Type", selected, typeKeys);
		if (selected >= 0) {
			if (typeNameProperty.stringValue != typeKeys [selected]) {
				Undo.RecordObject (param, "Change TypeName");
				typeNameProperty.stringValue = typeKeys [selected];
			}
		}
	}

	void DrawValueField() {
		var label = new GUIContent ("Value");

		switch (param.majorType.ToString()) {
		case "System.Boolean":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("boolValue"), label);
			break;
		case "System.Int32":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("intValue"), label);
			break;
		case "System.String":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("stringValue"), label);
			break;
		case "System.Double":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("doubleValue"), label);
			break;
		case "UnityEngine.Vector2":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("vector2Value"), label);
			break;
		case "UnityEngine.Vector3":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("vector3Value"), label);
			break;
		case "UnityEngine.Vector4":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("vector4Value"), label);
			break;
		case "UnityEngine.Quaternion":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("quaternionValue"), label);
			break;
		case "UnityEngine.Object":
			GenericObjectField(param);
			//EditorGUILayout.ObjectField (serializedObject.FindProperty ("objectValue"), param.minorType);
			break;
		}
	}

	void GenericObjectField(AnyParameter param)
	{
		Object currentValue = param.objectValue;
		var edited = EditorGUILayout.ObjectField ("Value", currentValue, param.minorType, true);
		if (currentValue != edited) {
			Undo.RecordObject (param, "Change "+param.minorType+" Value");
			param.objectValue = edited;
		}
	}

	void DrawActionButton() {
		GUILayout.FlexibleSpace ();
		int selected = 0;
		string[] options = new string[] {
			// this order must be the same as enum Action
			"...", "MoveUp", "MoveDown", "Delete",
		};
		selected = EditorGUILayout.Popup("", selected, options, GUILayout.MaxWidth(50));
		if  (selected > 0) {
			var action = ConvertPopupSelectionToAction(selected);
			if (OnParameterEvent != null) {
				OnParameterEvent (param, action);
			}
		}
	}

	// convert int (Popup selection) to Action type
	Action ConvertPopupSelectionToAction(int selection) {
		return (Action)(selection - 1);
	}
}

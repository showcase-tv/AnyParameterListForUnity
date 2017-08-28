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

	private AnyParameter _param;
	private bool _foldout = true;
	private GUIStyle _style;

	public delegate void OnParameterAction(AnyParameter _param, Action action);
	public event OnParameterAction OnParameterEvent;

	void OnEnable() {
		_style = new GUIStyle (EditorStyles.foldout);
		_style.fontStyle = FontStyle.Bold;
		_param = (AnyParameter)target;
	}

	public void OnInspectorGUIWithParent(AnyParameterListEditor parent) {
		OnInspectorGUI ();
	}

	public override void OnInspectorGUI()
	{
		
		GUILayout.BeginHorizontal ();
		_foldout = EditorGUILayout.Foldout(_foldout, _param.Title, true, _style);
		DrawActionButton ();
		GUILayout.EndHorizontal ();
		if (_foldout) {
			// start modifying
			serializedObject.Update();

			////// show parent for debug purpose only
			//EditorGUILayout.PropertyField (serializedObject.FindProperty ("parent"));
	
			// Type GUI
			DrawTypeNameField ();

			// Id GUI
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_id"));

			// Value GUI
			if (_param.MajorType != null) {
				DrawValueField ();
			}

			// comment GUI
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_comment"));
			// apply to object
			serializedObject.ApplyModifiedProperties();
		}
	}

	void DrawTypeNameField() {
		var typeNameProperty = serializedObject.FindProperty ("_typeName");
		var typeKeys = AnyParameter.TypeKeys;
		int selected = System.Array.IndexOf(typeKeys, typeNameProperty.stringValue);
		selected = EditorGUILayout.Popup("Type", selected, typeKeys);
		if (selected >= 0) {
			if (typeNameProperty.stringValue != typeKeys [selected]) {
				Undo.RecordObject (_param, "Change TypeName");
				typeNameProperty.stringValue = typeKeys [selected];
			}
		}
	}

	void DrawValueField() {
		var label = new GUIContent ("Value");

		switch (_param.MajorType.ToString()) {
		case "System.Boolean":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_boolValue"), label);
			break;
		case "System.Int32":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_intValue"), label);
			break;
		case "System.String":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_stringValue"), label);
			break;
		case "System.Double":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_doubleValue"), label);
			break;
		case "UnityEngine.Vector2":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_vector2Value"), label);
			break;
		case "UnityEngine.Vector3":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_vector3Value"), label);
			break;
		case "UnityEngine.Vector4":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_vector4Value"), label);
			break;
		case "UnityEngine.Quaternion":
			EditorGUILayout.PropertyField (serializedObject.FindProperty ("_quaternionValue"), label);
			break;
		case "UnityEngine.Object":
			GenericObjectField(_param);
			//EditorGUILayout.ObjectField (serializedObject.FindProperty ("objectValue"), _param.minorType);
			break;
		}
	}

	static void GenericObjectField(AnyParameter param)
	{
		Object currentValue = param.ObjectValue;
		var edited = EditorGUILayout.ObjectField ("Value", currentValue, param.MinorType, true);
		if (currentValue != edited) {
			Undo.RecordObject (param, "Change "+param.MinorType+" Value");
			param.ObjectValue = edited;
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
				OnParameterEvent (_param, action);
			}
		}
	}

	// convert int (Popup selection) to Action type
	Action ConvertPopupSelectionToAction(int selection) {
		return (Action)(selection - 1);
	}
}

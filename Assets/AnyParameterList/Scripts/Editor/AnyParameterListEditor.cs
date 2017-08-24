using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using APL;

[CustomEditor(typeof(APL.AnyParameterList))]
public class AnyParameterListEditor : Editor {

	private AnyParameterList parameterList;
	private List<Editor> editors = new List<Editor> ();
	private AnyParameter paramToDelete = null;
	private AnyParameter paramToMoveUp = null;
	private AnyParameter paramToMoveDown = null;

	void OnEnable() {
		parameterList = (AnyParameterList)target;
	}

	void OnDestroy() {
		//Debug.Log ("AnyParameterListEditor:OnDestroy called.");
		foreach (var editor in editors) {
			Object.DestroyImmediate (editor);
		}
		editors.Clear ();
	}

	void UpdateParameterEditors() {
		CreateNeededEditors ();

		RemoveUnneededEditors ();
	}

	void CreateNeededEditors() {
		foreach (var param in parameterList.Parameters) {
			var editor = EditorForParameter (param);
			if (editor == null) {
				//Debug.Log ("editor for parameter:"+param.Title+" not found.");
				editors.Add (CreateParameterEditor(param));
			}
		}
	}

	Editor CreateParameterEditor(AnyParameter param) {
		Editor previousEditor = null;
		Editor.CreateCachedEditor (param, typeof(AnyParameterEditor), ref previousEditor);
		var newEditor = (AnyParameterEditor)previousEditor;
		if (newEditor == null) {
			newEditor = (AnyParameterEditor)Editor.CreateEditor (param, typeof(AnyParameterEditor));
		}
		newEditor.OnParameterEvent += OnParameterAction;
		return newEditor;
	}

	void RemoveUnneededEditors() {
		// remove unneeded editors
		var editorsToDelete = new List<Editor> ();
		foreach (var editor in editors) {
			var param = parameterList.Parameters.Find (parameter => Object.ReferenceEquals (parameter, editor.target));
			if (param == null) {
				editorsToDelete.Add(editor);
			}
		}
		//Debug.Log ("editors.Count : " + editors.Count);
		//Debug.Log ("editorsToDelete.Count : " + editorsToDelete.Count);
		foreach (var editor in editorsToDelete) {
			//Debug.Log ("removing editor for :" + editor.target + "|" + ((AnyParameter)editor.target).Title);
			editors.Remove(editor);
			Object.DestroyImmediate (editor);
		}
	}

	Editor EditorForParameter(AnyParameter paramToFind) {
		foreach (var editor in editors) {
			if (Object.ReferenceEquals (editor.target, paramToFind)) {
				return editor;
			}
		}
		return null;
	}

	void DrawParametersInspectorGUI() {
		UpdateParameterEditors ();
		foreach(var param in parameterList.Parameters) {
			var editor = (AnyParameterEditor) EditorForParameter (param);
			EditorGUI.indentLevel += 1;
			editor.OnInspectorGUI();
			EditorGUI.indentLevel -= 1;
		}
		DeleteRequestedParam ();
		MoveUpRequestedParam ();
		MoveDownRequestedParam ();
	}

	void DeleteRequestedParam() {
		if (paramToDelete != null) {
			DeleteParameterViaEditor (parameterList, paramToDelete);
			paramToDelete = null;
		}
	}

	void MoveUpRequestedParam() {
		if (paramToMoveUp != null) {
			int index = parameterList.Parameters.IndexOf (paramToMoveUp);
			if (index > 0) {
				Undo.RecordObject (parameterList, "MoveUp Parameter");
				parameterList.Parameters.Remove (paramToMoveUp);
				parameterList.Parameters.Insert (index - 1, paramToMoveUp);
			} else {
				Debug.Log ("MoveUp not executable.");
			}
			paramToMoveUp = null;
		}
	}

	void MoveDownRequestedParam() {
		if (paramToMoveDown != null) {
			int index = parameterList.Parameters.IndexOf (paramToMoveDown);
			if (index >= 0 && index < parameterList.Parameters.Count -1) {
				Undo.RecordObject (parameterList, "MoveDown Parameter");
				parameterList.Parameters.Remove (paramToMoveDown);
				parameterList.Parameters.Insert (index + 1, paramToMoveDown);
			} else {
				Debug.Log ("MoveDown not executable.");
			}
			paramToMoveDown = null;
		}
	}


	void OnParameterAction(AnyParameter param, AnyParameterEditor.Action action) {
		switch (action) {
		case AnyParameterEditor.Action.Delete:
			paramToDelete = param;
			break;
		case AnyParameterEditor.Action.MoveUp:
			paramToMoveUp = param;
			break;
		case AnyParameterEditor.Action.MoveDown:
			paramToMoveDown = param;
			break;
		}
	}

	public override void OnInspectorGUI()
	{
		// start modifying
		serializedObject.Update();
		// comment GUI
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("comment"));
		// apply to object
		serializedObject.ApplyModifiedProperties();

		// parameters GUI
		DrawParametersInspectorGUI();

		if (GUILayout.Button ("Add New Parameter")) {
			Undo.RecordObject(parameterList, "Add New Parameter");
			AnyParameter newParam = parameterList.AddParameter ();
			Undo.RegisterCreatedObjectUndo (newParam, "Add New Parameter");
		}
	}

	// delete method, conforms to Undo
	public void DeleteParameterViaEditor(AnyParameterList paramList, AnyParameter param) {
		Undo.RecordObject(parameterList, "Delete Parameter");
		var index = paramList.Parameters.IndexOf (param);
		if (index < 0) {
			Debug.LogError ("DeleteParameter(): param<"+param.id+"> not found in AnyParameterList.");
			return;
		}
		paramList.Parameters.Remove (param);
		Undo.DestroyObjectImmediate (param);
	}
}

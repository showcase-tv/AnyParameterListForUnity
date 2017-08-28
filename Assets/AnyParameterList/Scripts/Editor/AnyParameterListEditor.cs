using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using APL;

[CustomEditor(typeof(APL.AnyParameterList))]
public class AnyParameterListEditor : Editor {

	private AnyParameterList _parameterList;
	private List<Editor> _editors = new List<Editor> ();
	private AnyParameter _paramToDelete = null;
	private AnyParameter _paramToMoveUp = null;
	private AnyParameter _paramToMoveDown = null;

	void OnEnable() {
		_parameterList = (AnyParameterList)target;
	}

	void OnDestroy() {
		//Debug.Log ("AnyParameterListEditor:OnDestroy called.");
		foreach (var editor in _editors) {
			Object.DestroyImmediate (editor);
		}
		_editors.Clear ();
	}

	void UpdateParameterEditors() {
		CreateNeededEditors ();

		RemoveUnneededEditors ();
	}

	void CreateNeededEditors() {
		foreach (var param in _parameterList.Parameters) {
			var editor = EditorForParameter (param);
			if (editor == null) {
				//Debug.Log ("editor for parameter:"+param.Title+" not found.");
				_editors.Add (CreateParameterEditor(param));
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
		// remove unneeded _editors
		var editorsToDelete = new List<Editor> ();
		foreach (var editor in _editors) {
			var param = _parameterList.Parameters.Find (parameter => Object.ReferenceEquals (parameter, editor.target));
			if (param == null) {
				editorsToDelete.Add(editor);
			}
		}
		//Debug.Log ("_editors.Count : " + _editors.Count);
		//Debug.Log ("editorsToDelete.Count : " + editorsToDelete.Count);
		foreach (var editor in editorsToDelete) {
			//Debug.Log ("removing editor for :" + editor.target + "|" + ((AnyParameter)editor.target).Title);
			_editors.Remove(editor);
			Object.DestroyImmediate (editor);
		}
	}

	Editor EditorForParameter(AnyParameter paramToFind) {
		foreach (var editor in _editors) {
			if (Object.ReferenceEquals (editor.target, paramToFind)) {
				return editor;
			}
		}
		return null;
	}

	void DrawParametersInspectorGUI() {
		UpdateParameterEditors ();
		foreach(var param in _parameterList.Parameters) {
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
		if (_paramToDelete != null) {
			DeleteParameterViaEditor (_parameterList, _paramToDelete);
			_paramToDelete = null;
		}
	}

	void MoveUpRequestedParam() {
		if (_paramToMoveUp != null) {
			int index = _parameterList.Parameters.IndexOf (_paramToMoveUp);
			if (index > 0) {
				Undo.RecordObject (_parameterList, "MoveUp Parameter");
				_parameterList.Parameters.Remove (_paramToMoveUp);
				_parameterList.Parameters.Insert (index - 1, _paramToMoveUp);
			} else {
				Debug.Log ("MoveUp not executable.");
			}
			_paramToMoveUp = null;
		}
	}

	void MoveDownRequestedParam() {
		if (_paramToMoveDown != null) {
			int index = _parameterList.Parameters.IndexOf (_paramToMoveDown);
			if (index >= 0 && index < _parameterList.Parameters.Count -1) {
				Undo.RecordObject (_parameterList, "MoveDown Parameter");
				_parameterList.Parameters.Remove (_paramToMoveDown);
				_parameterList.Parameters.Insert (index + 1, _paramToMoveDown);
			} else {
				Debug.Log ("MoveDown not executable.");
			}
			_paramToMoveDown = null;
		}
	}


	void OnParameterAction(AnyParameter param, AnyParameterEditor.Action action) {
		switch (action) {
		case AnyParameterEditor.Action.Delete:
			_paramToDelete = param;
			break;
		case AnyParameterEditor.Action.MoveUp:
			_paramToMoveUp = param;
			break;
		case AnyParameterEditor.Action.MoveDown:
			_paramToMoveDown = param;
			break;
		}
	}

	public override void OnInspectorGUI()
	{
		// start modifying
		serializedObject.Update();
		// comment GUI
		EditorGUILayout.PropertyField (serializedObject.FindProperty ("_comment"));
		// apply to object
		serializedObject.ApplyModifiedProperties();

		// parameters GUI
		DrawParametersInspectorGUI();

		if (GUILayout.Button ("Add New Parameter")) {
			Undo.RecordObject(_parameterList, "Add New Parameter");
			AnyParameter newParam = _parameterList.AddParameter ();
			Undo.RegisterCreatedObjectUndo (newParam, "Add New Parameter");
		}
	}

	// delete method, conforms to Undo
	public void DeleteParameterViaEditor(AnyParameterList paramList, AnyParameter param) {
		Undo.RecordObject(_parameterList, "Delete Parameter");
		var index = paramList.Parameters.IndexOf (param);
		if (index < 0) {
			Debug.LogError ("DeleteParameter(): param<"+param.Id+"> not found in AnyParameterList.");
			return;
		}
		paramList.Parameters.Remove (param);
		Undo.DestroyObjectImmediate (param);
	}
}

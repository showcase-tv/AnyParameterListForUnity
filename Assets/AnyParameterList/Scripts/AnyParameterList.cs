using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Features:
 *  - copy AnyParameterList component without problems.
 *  - support undo in editor, including adding/deleting _parameters.
 *  - scene/AssetBundle serialization ready.
 * 
 * Known Problem:
 *  - use more storage space than ideal. (but usually negligible)
 *  - use more serialization CPU time than ideal. (but usually negligible)
 * 
 * TODO:
 *  - add more parameter types.
 *  - cleaner inspector UI
 */

namespace APL {

	public class AnyParameterList : MonoBehaviour {

		[SerializeField]
		private List<AnyParameter> _parameters = new List<AnyParameter> ();

		[SerializeField, TextArea(1,10)]
		private string _comment;

		public string Comment {
			get { return _comment; }
			set { _comment = value; }
		}

		public List<AnyParameter> Parameters {
			get { return _parameters; }
		}

		public int Count {
			get { return _parameters.Count; }
		}

		public override int GetHashCode() {
			return base.GetHashCode() ^ _parameters.GetHashCode ();
		}

		void OnValidate() {
			var invalidParams = new List<AnyParameter> ();
			foreach (var param in _parameters) {
				if (param.Parent != this) {
					// this parameter is invalid, probably because it was copies from somewhere.
					invalidParams.Add(param);
				}
			}
			foreach (var param in invalidParams) {
				Debug.Log ("Renewing parameter<"+param.Title+"> because it must have been copied from somewhere.");
				RenewParameter(param);
			}
		}

		// replace invalid parameter with cloned instance.
		void RenewParameter(AnyParameter param) {
			
			int index = _parameters.IndexOf(param);
			_parameters.RemoveAt (index);
			var newParam = param.CloneToParent (this);
			Parameters.Insert (index, newParam);
		}

		public AnyParameter AddParameter() {
			var param = this.gameObject.AddComponent<AnyParameter> ();
			param.Setup (this);
			_parameters.Add (param);
			return param;
		}

		public void DeleteParameter(AnyParameter param) {
			var index = _parameters.IndexOf (param);
			if (index < 0) {
				Debug.LogError ("DeleteParameter(): param<"+param.Id+"> not found in AnyParameterList.");
				return;
			}
			_parameters.Remove (param);
			GameObject.DestroyImmediate (param);
		}

		public AnyParameter FindParameter(string id) {
			return _parameters.Find (param => (param.Id == id));
		}
	}

} // namespace APL

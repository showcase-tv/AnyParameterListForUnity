using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Features:
 *  - copy AnyParameterList component without problems.
 *  - support undo in editor, including adding/deleting parameters.
 *  - scene/AssetBundle serialization ready.
 * 
 * Known Problem:
 *  - use more storage space than ideal. (but usually negligible)
 *  - use more serialization CPU time than ideal. (but usually negligible)
 * 
 * TODO:
 *  - add more parameter types.
 *  - avoid public variables.
 *  - cleaner inspector UI
 *
 */

namespace APL {

	public class AnyParameterList : MonoBehaviour {

		[SerializeField]
		private List<AnyParameter> parameters = new List<AnyParameter> ();

		[SerializeField, TextArea(1,10)]
		public string comment;

		public List<AnyParameter> Parameters {
			get { return parameters; }
		}

		public int Count {
			get { return parameters.Count; }
		}

		public override int GetHashCode() {
			return base.GetHashCode() ^ parameters.GetHashCode ();
		}

		void OnValidate() {
			var invalidParams = new List<AnyParameter> ();
			foreach (var param in parameters) {
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
			
			int index = parameters.IndexOf(param);
			parameters.RemoveAt (index);
			var newParam = param.CloneToParent (this);
			Parameters.Insert (index, newParam);
		}

		public AnyParameter AddParameter() {
			var param = this.gameObject.AddComponent<AnyParameter> ();
			param.Setup (this);
			parameters.Add (param);
			return param;
		}

		public void DeleteParameter(AnyParameter param) {
			var index = parameters.IndexOf (param);
			if (index < 0) {
				Debug.LogError ("DeleteParameter(): param<"+param.id+"> not found in AnyParameterList.");
				return;
			}
			parameters.Remove (param);
			GameObject.DestroyImmediate (param);
		}
	}

} // namespace APL

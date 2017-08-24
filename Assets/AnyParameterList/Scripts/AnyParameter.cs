using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * To add a new value type:
 * 
 * 1. add an entry to TypeInfoTable
 * 2. If the type NOT inherited from Engine.Object:
 *   2.1. add a field of that type (not needed for types inherited Object class)
 *   2.2. add a copying value line to CloneToParent
 *   2.3. On AddParameterEditor class: add a new case to DrawValueField()
 */

namespace APL {
	[AddComponentMenu("")] // hide this class in "Add Component" menu.
	public class AnyParameter : MonoBehaviour {
		public struct TypeInfo {
			public string title;
			public System.Type majorType;
			public System.Type minorType;
			public TypeInfo(string title, System.Type majorType, System.Type minorType) {
				this.title = title;
				this.majorType = majorType;
				this.minorType = minorType;
			}
		}

		public static Dictionary<string, TypeInfo> TypeInfoTable = new Dictionary<string, TypeInfo>(){
			{ "System.Boolean", new TypeInfo("Boolean", typeof(System.Boolean), null) },
			{ "System.Int32", new TypeInfo("Int", typeof(System.Int32), null) },
			{ "System.String", new TypeInfo("String", typeof(System.String), null) },
			{ "System.Double", new TypeInfo("Double", typeof(System.Double), null) },
			{ "UnityEngine.Vector2", new TypeInfo("Vector2", typeof(UnityEngine.Vector2), null) },
			{ "UnityEngine.Vector3", new TypeInfo("Vector3", typeof(UnityEngine.Vector3), null) },
			// not implemented: { "UnityEngine.Vector4", new TypeInfo("Vector4", typeof(UnityEngine.Vector4), null) },
			// not implemented: { "UnityEngine.Quaternion", new TypeInfo("Quaternion", typeof(UnityEngine.Quaternion), null) },
			{ "UnityEngine.Object", new TypeInfo("UnityEngine.Object", typeof(UnityEngine.Object), null) },
			{ "UnityEngine.GameObject", new TypeInfo("GameObject", typeof(UnityEngine.Object), typeof(UnityEngine.GameObject)) },
			{ "UnityEngine.Texture2D", new TypeInfo("Texture2D", typeof(UnityEngine.Object), typeof(UnityEngine.Texture2D)) },
		};

		public static string[] TypeKeys {
			get {
				var types = AnyParameter.TypeInfoTable.Keys;
				var typeArray = new string[ types.Count ];
				types.CopyTo (typeArray, 0);
				return typeArray;
			}
		}

		[SerializeField]
		AnyParameterList parent; // 親を覚えておく

		public AnyParameterList Parent {
			get {
				return parent;
			}
			set {
				this.parent = value;
			}
		}

		public System.Type majorType {
			get {
				if (typeName != null && TypeInfoTable.ContainsKey (typeName)) {
					return TypeInfoTable [typeName].majorType;
				}
				return null;
			}
		}

		public System.Type minorType {
			get {
				if (typeName != null && TypeInfoTable.ContainsKey (typeName)) {
					var info = TypeInfoTable [typeName];
					if (info.minorType != null) {
						return info.minorType;
					}
					return info.majorType;
				}
				return null;
			}
		}

		[SerializeField]
		public string id;

		[SerializeField]
		public string typeName;

		[SerializeField, TextArea(1,10)]
		public string comment;

		// values
		[SerializeField]
		public bool boolValue;

		[SerializeField]
		public int intValue;

		[SerializeField]
		private string stringValue;

		[SerializeField]
		public double doubleValue;

		[SerializeField]
		public Vector2 vector2Value;
		[SerializeField]
		public Vector3 vector3Value;
		[SerializeField]
		public Vector4 vector4Value;
		[SerializeField]
		public Quaternion quaternionValue;

		[SerializeField]
		public UnityEngine.Object objectValue;


		public void Setup(AnyParameterList parent) {
			this.parent = parent;
			hideFlags = HideFlags.HideInInspector;
		}

		void OnValidate() {
			//Debug.Log ("OnValidate called.");
			CleanReferences ();
			hideFlags = HideFlags.HideInInspector;
		}

		void CleanReferences() {
			if (majorType != typeof(UnityEngine.Object)) {
				objectValue = null;
			}
		}

		public override int GetHashCode() {
			return id.GetHashCode () ^ typeName.GetHashCode ();
		}

		public AnyParameter CloneToParent(AnyParameterList paramList) {
			var newParam = paramList.gameObject.AddComponent<AnyParameter> ();
			newParam.Setup (paramList);
			newParam.id = id;
			newParam.typeName = typeName;
			newParam.comment = comment;
			// copy values
			newParam.boolValue = boolValue;
			newParam.intValue = intValue;
			newParam.stringValue = stringValue;
			newParam.doubleValue = doubleValue;
			newParam.vector2Value = vector2Value;
			newParam.vector3Value = vector3Value;
			newParam.vector4Value = vector4Value;
			newParam.quaternionValue = quaternionValue;
			newParam.objectValue = objectValue;
			return newParam;
		}

		public string Title {
			get { return "" + id + " (" + typeName + ")"; }
		}
	}
} // namespace APL
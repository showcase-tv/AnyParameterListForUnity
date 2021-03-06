﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * To add a new value type:
 * 
 * 1. Add an entry to TypeInfoTable
 * 2. If the type is NOT inherited from Engine.Object:
 *   2.1. Add a field of that type (not needed for types inherited Object class)
 *   2.2. Add a line about copying a value to CloneToParent
 *   2.3. On AnyParameterEditor class: add a new case to DrawValueField()
 *   2.4. Add an assertion on AnyParameterTest
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
			{ "System.Single", new TypeInfo("Float", typeof(System.Single), null) },
			{ "UnityEngine.Vector2", new TypeInfo("Vector2", typeof(UnityEngine.Vector2), null) },
			{ "UnityEngine.Vector3", new TypeInfo("Vector3", typeof(UnityEngine.Vector3), null) },
			// not implemented: { "UnityEngine.Vector4", new TypeInfo("Vector4", typeof(UnityEngine.Vector4), null) },
			// not implemented: { "UnityEngine.Quaternion", new TypeInfo("Quaternion", typeof(UnityEngine.Quaternion), null) },
			{ "UnityEngine.Color", new TypeInfo("Color", typeof(UnityEngine.Color), null) },
			{ "UnityEngine.Rect", new TypeInfo("Rect", typeof(UnityEngine.Rect), null) },
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
		private AnyParameterList _parent; // 親を覚えておく

		public AnyParameterList Parent {
			get {
				return _parent;
			}
			set {
				this._parent = value;
			}
		}

		public System.Type MajorType {
			get {
				if (_typeName != null && TypeInfoTable.ContainsKey (_typeName)) {
					return TypeInfoTable [_typeName].majorType;
				}
				return null;
			}
		}

		public System.Type MinorType {
			get {
				if (_typeName != null && TypeInfoTable.ContainsKey (_typeName)) {
					var info = TypeInfoTable [_typeName];
					if (info.minorType != null) {
						return info.minorType;
					}
					return info.majorType;
				}
				return null;
			}
		}

		[SerializeField]
		private string _id;
		public string Id {
			get { return _id; }
			set { _id = value; }
		}

		[SerializeField]
		private string _typeName;
		public string TypeName {
			get { return _typeName; }
			set {
				_typeName = value;
				CleanReferences ();
			}
		}

		[SerializeField, TextArea(1,10)]
		private string _comment;
		public string Comment {
			get { return _comment; }
			set { _comment = value; }
		}

		// values
		[SerializeField]
		private bool _boolValue;
		public bool BoolValue {
			get { return _boolValue; }
			set { _boolValue = value; }
		}

		[SerializeField]
		private int _intValue;
		public int IntValue {
			get { return _intValue; }
			set { _intValue = value; }
		}

		[SerializeField]
		private string _stringValue;
		public string StringValue {
			get { return _stringValue; }
			set { _stringValue = value; }
		}

		[SerializeField]
		private float _floatValue;
		public float FloatValue {
			get { return _floatValue; }
			set { _floatValue = value; }
		}

		[SerializeField]
		private double _doubleValue;
		public double DoubleValue {
			get { return _doubleValue; }
			set { _doubleValue = value; }
		}

		[SerializeField]
		private Vector2 _vector2Value;
		public Vector2 Vector2Value {
			get { return _vector2Value; }
			set { _vector2Value = value; }
		}

		[SerializeField]
		private Vector3 _vector3Value;
		public Vector3 Vector3Value {
			get { return _vector3Value; }
			set { _vector3Value = value; }
		}

		[SerializeField]
		private Vector4 _vector4Value;
		public Vector4 Vector4Value {
			get { return _vector4Value; }
			set { _vector4Value = value; }
		}

		[SerializeField]
		private Quaternion _quaternionValue;
		public Quaternion QuaternionValue {
			get { return _quaternionValue; }
			set { _quaternionValue = value; }
		}

		[SerializeField]
		private Color _colorValue;
		public Color ColorValue {
			get { return _colorValue; }
			set { _colorValue = value; }
		}

		[SerializeField]
		private Rect _rectValue;
		public Rect RectValue {
			get { return _rectValue; }
			set { _rectValue = value; }
		}

		[SerializeField]
		private UnityEngine.Object _objectValue;
		public UnityEngine.Object ObjectValue {
			get { return _objectValue; }
			set { _objectValue = value; }
		}


		public void Setup(AnyParameterList parent) {
			this._parent = parent;
			hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		void OnValidate() {
			//Debug.Log ("OnValidate called.");
			CleanReferences ();
			hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
		}

		void CleanReferences() {
			if (MajorType != typeof(UnityEngine.Object)) {
				_objectValue = null;
			}
		}

		public override int GetHashCode() {
			return _id.GetHashCode () ^ _typeName.GetHashCode ();
		}

		public AnyParameter CloneToParent(AnyParameterList paramList) {
			var newParam = paramList.gameObject.AddComponent<AnyParameter> ();
			newParam.Setup (paramList);
			newParam.Id = _id;
			newParam._typeName = _typeName;
			newParam._comment = _comment;
			// copy values
			newParam.BoolValue = _boolValue;
			newParam.IntValue = _intValue;
			newParam.StringValue = _stringValue;
			newParam.FloatValue = _floatValue;
			newParam.DoubleValue = _doubleValue;
			newParam.Vector2Value = _vector2Value;
			newParam.Vector3Value = _vector3Value;
			newParam.Vector4Value = _vector4Value;
			newParam.QuaternionValue = _quaternionValue;
			newParam.ColorValue = _colorValue;
			newParam.RectValue = _rectValue;
			newParam.ObjectValue = _objectValue;
			return newParam;
		}

		public string Title {
			get { return "" + _id + " (" + _typeName + ")"; }
		}
	}
} // namespace APL
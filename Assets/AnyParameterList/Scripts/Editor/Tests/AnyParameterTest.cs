using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using APL;

public class AnyParameterTest {

	private AnyParameterList CreateList() {
		var gameObject = new GameObject ();
		var parameterList = gameObject.AddComponent<AnyParameterList> ();
		return parameterList;
	}

	private AnyParameter CreatedParameter(string id) {
		var paramList = CreateList ();
		var param = paramList.AddParameter ();
		param.Id = id;
		return param;
	}

	[Test]
	public void HideFlagTest() {
		var param = CreatedParameter ("hoge");
		Assert.AreEqual (param.hideFlags & HideFlags.HideInInspector, HideFlags.HideInInspector);
	}

	[Test]
	public void ValuesTest() {
		var param = CreatedParameter ("hoge");
		param.BoolValue = true;
		Assert.IsTrue (param.BoolValue);
		param.BoolValue = false;
		Assert.IsFalse (param.BoolValue);
		param.IntValue = 5555;
		Assert.AreEqual (param.IntValue, 5555);
		param.DoubleValue = 123.45;
		Assert.AreEqual (param.DoubleValue, 123.45);
		param.FloatValue = 987.65f;
		Assert.AreEqual (param.FloatValue, 987.65f);
		param.StringValue = "HogeMoge";
		Assert.AreEqual (param.StringValue, "HogeMoge");
		param.Vector2Value = new Vector2 (1.1f, 2.2f);
		Assert.AreEqual (param.Vector2Value, new Vector2 (1.1f, 2.2f));
		param.Vector3Value = new Vector3 (1.1f, 2.2f, 3.3f);
		Assert.AreEqual (param.Vector3Value, new Vector3 (1.1f, 2.2f, 3.3f));
		param.Vector4Value = new Vector4 (1.1f, 2.2f, 3.3f, 4.4f);
		Assert.AreEqual (param.Vector4Value, new Vector4 (1.1f, 2.2f, 3.3f, 4.4f));
		param.QuaternionValue = new Quaternion (1.1f, 2.2f, 3.3f, 4.4f);
		Assert.AreEqual (param.QuaternionValue, new Quaternion (1.1f, 2.2f, 3.3f, 4.4f));
		param.ColorValue = new Color (0.1f, 0.2f, 0.3f, 0.4f);
		Assert.AreEqual (param.ColorValue, new Color (0.1f, 0.2f, 0.3f, 0.4f));
		param.RectValue = new Rect (10.0f, 20.0f, 30.0f, 40.0f);
		Assert.AreEqual (param.RectValue, new Rect (10.0f, 20.0f, 30.0f, 40.0f));
	}

	[Test]
	public void ObjectsTest() {
		var obj = new GameObject ();
		var param = CreatedParameter ("hoge");
		param.ObjectValue = obj;
		Assert.AreSame (param.ObjectValue, obj);

		var gameObject2 = GameObject.Instantiate (param.Parent.gameObject);
		var paramList = gameObject2.GetComponent<AnyParameterList> ();
		var param2 = paramList.FindParameter ("hoge");
		Assert.AreNotSame(param, param2);
		Assert.AreSame (param.ObjectValue, param2.ObjectValue);
	}

	[Test]
	public void TypeNameTest() {
		var param = CreatedParameter ("hoge");
		param.TypeName = "System.String";
		Assert.AreEqual (param.MajorType, typeof(System.String));
		Assert.AreEqual (param.MinorType, typeof(System.String));
		param.TypeName = "UnityEngine.Object";
		Assert.AreEqual (param.MajorType, typeof(UnityEngine.Object));
		Assert.AreEqual (param.MinorType, typeof(UnityEngine.Object));
		param.TypeName = "UnityEngine.Texture2D";
		Assert.AreEqual (param.MajorType, typeof(UnityEngine.Object));
		Assert.AreEqual (param.MinorType, typeof(UnityEngine.Texture2D));
	}

	[Test]
	public void CleanReferenceTest() {
		var obj = new GameObject ();
		var param = CreatedParameter ("hoge");
		param.TypeName = "UnityEngine.Object";
		param.ObjectValue = obj;
		Assert.AreSame (param.ObjectValue, obj);

		param.TypeName = "System.String";
		Assert.IsNull (param.ObjectValue);
	}
}
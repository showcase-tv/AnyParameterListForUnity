using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using APL;

public class AnyParameterListTest {

	private AnyParameterList CreateList() {
		var gameObject = new GameObject ();
		var parameterList = gameObject.AddComponent<AnyParameterList> ();
		return parameterList;
	}

	[Test]
	public void AddToGameObjectTest() {
		var gameObject = new GameObject ();
		var parameterList = gameObject.AddComponent<AnyParameterList> ();

		Assert.NotNull (parameterList);
		Assert.AreEqual (parameterList.gameObject, gameObject);
	}

	[Test]
	public void RemoveFromGameObjectTest() {
		var parameterList = CreateList ();
		var gameObject = parameterList.gameObject;
		Object.DestroyImmediate (parameterList);

		Assert.IsNull (gameObject.GetComponent<AnyParameterList> ());
	}

	[Test]
	public void AddParameterTest() {
		var parameterList = CreateList ();
		var param = parameterList.AddParameter ();
		Assert.NotNull (param);
		param.Id = "hoge";

		Assert.AreSame(parameterList.FindParameter ("hoge"), param);
	}

	[Test]
	public void DeleteParameterTest() {
		var parameterList = CreateList ();
		var param = parameterList.AddParameter ();
		param.Id = "hoge";
		parameterList.DeleteParameter (param);

		Assert.IsNull (parameterList.FindParameter ("hoge"));
	}

	[Test]
	public void FindParameterTest() {
		var parameterList = CreateList ();
		var param = parameterList.AddParameter ();
		param.Id = "hoge";
		var param2 = parameterList.AddParameter ();
		param2.Id = "foo";
		Assert.AreSame(parameterList.FindParameter ("hoge"), param);
		Assert.AreSame(parameterList.FindParameter ("foo"), param2);
		Assert.IsNull (parameterList.FindParameter ("hog"));
	}
}

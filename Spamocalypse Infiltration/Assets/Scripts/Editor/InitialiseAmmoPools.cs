using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Editor class that initialises an ammo pool.
/// </summary>

[CustomEditor(typeof(AmmoPool))]
public class InitialiseAmmoPools: Editor {

	public override void OnInspectorGUI()
	{
		AmmoPool pool = target as AmmoPool;

		EditorGUI.BeginChangeCheck();
		pool.ammoPrefab = EditorGUILayout.ObjectField("Prefab", pool.ammoPrefab, typeof(GameObject), false) as GameObject;
		pool.maxAmmo = EditorGUILayout.IntField("Max Ammo", pool.maxAmmo);
		EditorGUILayout.Space ();
		EditorGUI.EndChangeCheck();

		if (GUILayout.Button("Create Ammo"))
		{
			pool.InstantiatePool();
		}

	}
}

using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Editor class to create a new instance of DeathSounds.
/// </summary>
public class ManageDeathSounds : EditorWindow {

	[MenuItem("Assets/Create/DeathSounds")]
	public static void CreateAsset()
	{
		DeathSounds asset = ScriptableObject.CreateInstance<DeathSounds>();

		AssetDatabase.CreateAsset(asset, "Assets/NewDeathSounds.asset");
		AssetDatabase.SaveAssets();

		Selection.activeObject = asset;
	}
}

using UnityEngine;
using UnityEditor;

public class MakePlayerVoice : EditorWindow {

	[MenuItem("Assets/Create/PlayerVoice")]
	public static void CreateAsset()
	{
		PlayerVoiceList asset = ScriptableObject.CreateInstance<PlayerVoiceList>();

		AssetDatabase.CreateAsset(asset, "Assets/Sound/" + asset.assetName + ".asset");
		AssetDatabase.SaveAssets();

		Selection.activeObject = asset;
	}
}

using UnityEngine;
using UnityEditor;

/// <summary>
/// Utility class to change the scale of an object in one click.
/// </summary>

[CustomEditor(typeof(ChangeScale))]
public class ChangeScaleEditor : Editor 
{

	float scaleFactor = 1;
	bool scaled;

	public override void OnInspectorGUI() 
	{
		ChangeScale transform = target as ChangeScale;
		scaleFactor = EditorGUILayout.FloatField ("New scale factor", scaleFactor);

		if (GUILayout.Button("Scale"))
		{
			transform.Scale(scaleFactor);
		}
	}
}

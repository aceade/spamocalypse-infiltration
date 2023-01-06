using UnityEngine;
using UnityEditor;

/// <summary>
/// Gathers the sun.
/// </summary>

[CustomEditor(typeof(Sun))]
public class GatherSun : Editor {

	bool showLightees;

	public override void OnInspectorGUI()
	{
		Sun theSun = target as Sun;
		if (GUILayout.Button("Gather Calculations"))
		{
			theSun.GatherLightees();
		}

		showLightees = GUILayout.Toggle(showLightees, "Show lightees");

		if (showLightees)
		{
			foreach(CalculateLight lightee in theSun.lightees) 
			{
				EditorGUILayout.LabelField(lightee.name);
			}
		}

		EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();
		theSun.maxRange = EditorGUILayout.FloatField("Maximum sun range", theSun.maxRange);
		EditorGUI.EndChangeCheck();
	}
}

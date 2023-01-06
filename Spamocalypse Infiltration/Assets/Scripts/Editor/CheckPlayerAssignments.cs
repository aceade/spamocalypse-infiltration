using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AssignPlayer))]
public class CheckPlayerAssignments : Editor {

	public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Assign"))
        {
            (target as AssignPlayer).Assign();
        }

    }
}

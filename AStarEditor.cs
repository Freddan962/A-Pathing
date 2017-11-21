using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AStar))]
public class AStarEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		AStar generator = (AStar)target;
		if (GUILayout.Button("Reset"))
		{
			/*if (EditorApplication.isPlaying)
				generator.reset();*/
		}
	}
}

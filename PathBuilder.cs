using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour {

	[Header("Status")]
	[Space(3)]
	public bool FinishedBuildingPath;

	[Header("Linking")]
	[Space(3)]
	public AStar AStar;
	public Grid Grid;
	public Transform PathContainer;

	[Header("Prefabs")]
	[Space(3)]
	public Transform PathObject;

	[Header("Timers")]
	[Space(3)]
	public float UpdateTimer = 1;
	public float CurrentUpdateTimer;

	public void Update()
	{
		CurrentUpdateTimer += Time.deltaTime;
		if (CurrentUpdateTimer > UpdateTimer)
		{
			checkForPathGeneration();
			CurrentUpdateTimer = 0;
		}
	}

	private void checkForPathGeneration()
	{
		if (!FinishedBuildingPath && AStar.FinishedPathing)
			generatePath();
	}

	private void generatePath()
	{
		List<Vector2> pathPoints = AStar.GeneratedPath;

		foreach (Vector2 pos in pathPoints)
			GameObject.Instantiate(PathObject, Grid.gridPositionToWorldPosition(pos), Quaternion.identity, PathContainer);

		FinishedBuildingPath = true;
	}
}


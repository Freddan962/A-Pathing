using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar : MonoBehaviour {
	[Header("Status")]
	[Space(3)]
	//Finished with pathing status
	public bool FinishedPathing = false;

	//The grid that we base the AStar pathing off
	public Grid Grid;

	//Keeps track of our system's performance
	[Header("Performance")]
	[Space(3)]
	public int PathingTilesPerUpdateTimerFilled;
	public float UpdateTimerStart;
	public float UpdateTimerCurrent;

	//Debug so we can see the A* pathing in works
	[Header("Debugging")]
	[Space(3)]
	public bool Debugging = false;
	public GameObject DebugObject;
	public Transform DebugObjectParent;
	private Dictionary<Vector2, GameObject> DebugObjectDict = new Dictionary<Vector2, GameObject>();
	
	//The objects we move between
	[Header("Object Linking")]
	[Space(3)]
	public GameObject PathingStartObject;
	public GameObject PathingEndTarget;

	//The objects we move between
	[Header("Information")]
	[Space(3)]
	public Vector2 StartingGridPosition;
	public Vector2 TargetGridPosition;
	public Vector2 ActivePathingPosition; 	//The position that we are currently attempting to path from

	[Header("Result")]
	[Space(3)]
	public List<Vector2> GeneratedPath = new List<Vector2>();

	//Tiles that are open respectively closed for consideration when calculating pathing
	private List<Step> _open = new List<Step>();
	private List<Step> _closed = new List<Step>();
	private bool _finishedBacktracking = false;

	private void Start()
	{
		//Prepare pathing start and end positions
		StartingGridPosition = Grid.worldPositionToGridPosition(PathingStartObject.transform.position);
		TargetGridPosition = Grid.worldPositionToGridPosition(PathingEndTarget.transform.position);

		//Update self position to that of the starting target
		this.transform.position = PathingStartObject.transform.position;

		//Start off by adding self to the closed list, we can't move to a position we're at!
		Step step = new Step();
		step.Position = Grid.worldPositionToGridPosition(this.transform.position);
		_closed.Add(step);

		//Only add to grid if there's not already a tile at this part of the map as we cant to generate from 
		//a tile point and not always a empty position.
		if (!Grid.isOccupied(step.Position))
			Grid.addEmptyTile(step.Position);

		//Add the starting position to the open list
		step = new Step();
		step.Position = StartingGridPosition;
		insertInOpenSteps(step);
		_open.Add(step);

		
		//We start off by trying to path from our own position
		ActivePathingPosition = Grid.worldPositionToGridPosition(this.transform.position);

		//Addd the end position to the open list
		step = new Step();
		step.Position = ActivePathingPosition;
		insertInOpenSteps(step);
		_open.Add(step);

		//Set the starting pathing position
		StartingGridPosition = ActivePathingPosition;
	}

	private void Update()
	{
		//When we have finished backtracking our path there's no need to keep generating one
		if (_finishedBacktracking)
			return;

		//In the inspector we can set if we can to slow down the pathing generation
		//for the sake of saving resources or having a easier time visually debugging
		UpdateTimerCurrent += Time.deltaTime;
		if (UpdateTimerCurrent < UpdateTimerStart)
			return;
		else
			UpdateTimerCurrent = 0;

		//Keep calculating path links while there are open paths left, or until we reach our PathingTilesPerFrameLimit
		int i = 0;
		while (_open.Count > 0)
		{
			if (i >= PathingTilesPerUpdateTimerFilled)
				break;

			performAStarPathing();

			//We check if we're ready to backtrack, and if so we do backtrack path
			if (TargetGridPosition == ActivePathingPosition && !_finishedBacktracking)
			{
				backtrackGeneratedPath();
				FinishedPathing = true;
				_finishedBacktracking = true;
				return;
			}

			i++;
		}
	}

	private void backtrackGeneratedPath()
	{
		//Sets step to the final step which collided with the target position
		Step step = _closed.First((v) => v.Position == ActivePathingPosition);

		//Add starting position
		GeneratedPath.Add(StartingGridPosition);

		//Keep backtracking every parent in the path chain til we reach our starting position
		while (step.ParentStep != null)
		{
			Step currentStep = step;

			if (Debugging)
				Destroy(DebugObjectDict[currentStep.Position]);

			GeneratedPath.Add(currentStep.Position);
			step = currentStep.ParentStep;
		}

		//Add ending position
		GeneratedPath.Add(ActivePathingPosition);
	}

	private void performAStarPathing()
	{
		//Aka the step that will take us the fastest to our goal
		Step currentStep = _open[0];
		//currentStep.Score.Total = currentStep.Score.Movement + currentStep.Score.Heuristics;

		//Lock the closest step
		_closed.Add(currentStep);
		_open.Remove(currentStep);

		List<Vector2> adjacentSteps = Grid.getAdjacentWalkableTilePositions(currentStep.Position);
		//Iterate over each adjacent step from the current step
		foreach (Vector2 v in adjacentSteps)
		{
			Step step = new Step(v);

			//If the step already has been closed we continue
			if (_closed.Contains(step))
				continue;

			//Calculate the moveCost for the currentStep to this adjacentStep
			int moveCost = costToMoveFromStep(currentStep.Position, step.Position);

			//Check if this adjacent step is open
			bool containsCoordinates = false;
			foreach (Step openStep in _open)
				if (openStep.Position.x == step.Position.x && openStep.Position.y == step.Position.y)
					containsCoordinates = true;

			//If the adjacent step is open
			if (containsCoordinates)
			{
				//Then we update it's movement cost and re-arrange the order array
				step = _open[_open.IndexOf(step)];
				if (currentStep.Score.Movement + moveCost < step.Score.Movement)
				{
					step.Score.Movement = currentStep.Score.Movement + moveCost;
					step.Score.Total = step.Score.Movement + step.Score.Heuristics; //Tillagt nu
					orderOpenSteps();
				}
			}
			else //If the adjacent step is not open yet, we add the step to the open list
			{
				step.ParentStep = currentStep;
				step.Score.Movement = currentStep.Score.Movement + moveCost;
				step.Score.Heuristics = computeHeuristicsScoreFromCoord(step.Position, TargetGridPosition);
				step.Score.Total = step.Score.Movement + step.Score.Heuristics;
				insertInOpenSteps(step);
			}

			//We occupy this tile in memory but not visually to prevent duplicate paths
			if (!Grid.isOccupied(step.Position))
				Grid.addEmptyTile(step.Position);
		}

		//Responsible for creating the visible debugging tiles
		if (Debugging)
		{
			if (!DebugObjectDict.ContainsKey(currentStep.Position))
			{
				GameObject debugObj = Instantiate(DebugObject, Grid.gridPositionToWorldPosition(currentStep.Position), Quaternion.identity, DebugObjectParent);
			
				DebugObjectDict.Add(currentStep.Position, debugObj);

				PathDebugText textModule = debugObj.GetComponent<PathDebugText>();
				textModule.TextTotal.text = currentStep.Score.Total.ToString();
				textModule.TextHeuristics.text = currentStep.Score.Heuristics.ToString();
				textModule.TextMovement.text = currentStep.Score.Movement.ToString();
			}
		}

		this.transform.position = Grid.gridPositionToWorldPosition(currentStep.Position);
		ActivePathingPosition = currentStep.Position;
	}

	//Inserts a step into the _open list and order list after it's total score
	private void insertInOpenSteps(Step steps)
	{
		_open.Add(steps);
		orderOpenSteps();
	}

	//Order _open after every element's total pathing score.
	private void orderOpenSteps()
	{
		//Ascending [0, 15, 31..]
		_open = _open.OrderBy((step) => step.Score.Total).ToList();
	}

	private int computeHeuristicsScoreFromCoord(Vector2 fromPos, Vector2 toPos)
	{
		return Mathf.Abs((int)toPos.x - (int)fromPos.x) + Mathf.Abs((int)toPos.y - (int)fromPos.y);
	}

	//Since we don't have different terrain types, e.g swamps, diagonal movement bla bla bla the movement cost is always the same.
	private int costToMoveFromStep(Vector2 fromPos, Vector2 toPos)
	{
		return 1;
	}
}

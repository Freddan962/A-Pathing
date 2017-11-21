using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step {
	
	public Vector2 Position;
	public Step ParentStep;
	public PathScore Score;

	public Step()
	{
		Position = Vector2.zero;
		Score = new PathScore();
	}

	public Step(Vector2 position)
	{
		Position = position;
		Score = new PathScore();
	}
}

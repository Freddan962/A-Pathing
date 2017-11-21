using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	The A* algorithm consists of two things that modifies the path scoring (aka movement cost)
	G (Movement) = cost per tile (in this case it's one per tile)
	H (Heuristics) = Estimated movement cost from starting position to destination
	T Total =  cost off each tile can be computed as F which is equal to H + G
*/

public class PathScore {
	public int Total;
	public int Movement;
	public int Heuristics;

	public PathScore() : this(0, 0) { }

	public PathScore(int movement, int heuristics)
	{
		Movement = movement;
		Heuristics = heuristics;
		Total = Movement + heuristics;
	}
}

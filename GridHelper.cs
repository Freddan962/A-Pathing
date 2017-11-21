using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridHelper {

	//Might not be functioning as intended
	public static List<Vector2> calculateWallGridPositions(int startPositionX, int startPositionY, int endPositionX, int endPositionY)
	{
		List<Vector2> positions = new List<Vector2>();

		for (int x = startPositionX; x <= endPositionX; x++)
			for (int y = startPositionY; y <= endPositionY; y++)
				if (x == startPositionX || y == startPositionY || x == endPositionX || y == endPositionY)
					positions.Add(new Vector2(x, y));

		return positions;
	}

	public static List<Vector2> calculateDoorGridPositions(int startPositionX, int startPositionY, int endPositionX, int endPositionY)
	{
		List<Vector2> wallGridPositions = GridHelper.calculateWallGridPositions(startPositionX, startPositionY, endPositionX, endPositionY);

		wallGridPositions = wallGridPositions.Where((position) => position.x != startPositionX && position.y != startPositionY 
													|| position.x != endPositionX && position.y != endPositionY).ToList();  

		return wallGridPositions;
	}
}

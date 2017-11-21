using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

	public Vector2 TileSize = new Vector3(5, 5, 5);
	private Dictionary<Vector2, GameObject> _contents = new Dictionary<Vector2, GameObject>();

	public void Clear()
	{
		_contents.Clear();
	}

	public Dictionary<Vector2, GameObject> getContents()
	{
		return _contents;
	}

	public bool isGridRangeOccupied(int startX, int startY, int width, int height)
	{
		int borderPaddingBetweenRooms = 1;

		for (int x = startX - borderPaddingBetweenRooms; x <= startX + width + borderPaddingBetweenRooms; x++)
			for (int y = startY - borderPaddingBetweenRooms; y <= startY + height + borderPaddingBetweenRooms; y++)
				if (_contents.ContainsKey(new Vector2(x, y)))
					return true;

		return false;
	}

	/* 
		1 2 3
		4 X 6       X = Player, numbers = tile position
		7 8 9 		here 2, 4, 6 and 8 would be free if nothing occupies them.
	*/

	public List<Vector2> getAdjacentWalkableTilePositions(Vector2 position)
	{
		List<Vector2> adjacentFreePositions = new List<Vector2>();
		Vector2[] positions = new Vector2[4];

		positions[0] = position + new Vector2(0, 1); //Top
		positions[1] = position + new Vector2(0, -1); //Bottom
		positions[2] = position + new Vector2(-1, 0); //Left
		positions[3] = position + new Vector2(1, 0); //Right

		foreach (Vector2 adjacentPos in positions)
			if (!isOccupied(adjacentPos))
				adjacentFreePositions.Add(adjacentPos);
	
		return adjacentFreePositions;
	}

	public bool isOccupied(Vector2 position)
	{
		if (_contents.ContainsKey(position))
			return true;

		return false;
	}

	public Vector2 worldPositionToGridPosition(Vector3 worldPosition)
	{
		Vector2 gridPosition = Vector2.zero;

		gridPosition.x = (int)(worldPosition.x/TileSize.x);
		gridPosition.y = (int)(worldPosition.z/TileSize.y);

		return gridPosition;
	}

	public Vector3 gridPositionToWorldPosition(Vector2 gridPosition)
	{
		Vector3 worldPosition = Vector3.zero;

		worldPosition.x = (int)(gridPosition.x * TileSize.x);
		worldPosition.y = 0;
		worldPosition.z = (int)(gridPosition.y * TileSize.y);

		return worldPosition;
	}

	public void addTile(Vector2 key, GameObject tile)
	{
		_contents.Add(key, tile);
	}

	public void addEmptyTile(Vector2 key)
	{
		_contents.Add(key, default(GameObject));
	}

	public void removeTile(Vector2 key)
	{
		if (!_contents.ContainsKey(key))
			return;

		_contents.Remove(key);
	}

	public GameObject getTile(Vector2 key)
	{
		if (_contents.ContainsKey(key))
			return _contents[key];
		else
			return null;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HexagonsCreator 
{
	public static void CreateZones(Transform root, Hexagon [] prefabs, float size)
	{
		
		// Settings.HexagonsPrefab.

		// 1. Create a trio of hexagons
		// 2. Choose one hexagon from the trio to start the loop
		// [Loop] While the count of hexagons to create is not reached :
		//	a. Create an hexagon that must have two neighbourgs AND doesn't reach the nLine max / nCol max.
		//	b. If no hexagon can not be created this way, take another created hexagon.

		const int N_HEXAGONS = 21;
		const int MAX_COL = N_HEXAGONS * 2;
		const int MAX_ROW = N_HEXAGONS * 2;
		Hexagon[,] matrix = new Hexagon[MAX_COL, MAX_ROW];
		Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>> stillPossible = new Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>>();

		CreateTrio(root, prefabs, stillPossible, matrix, MAX_COL, MAX_ROW);

		Hexagon current;
		Hexagon.NeighbourgPlace place;

		int nHexagons = 3;
		while (nHexagons < N_HEXAGONS)
		{
			if(!TakeRandomHexagonAndPlace(stillPossible, out current, out place))
				break;
			
			Hexagon newHexagon = CreateRandomZonePrefab(root, prefabs);
			PlaceRelativeTo(newHexagon, current, place, stillPossible, matrix, MAX_COL, MAX_ROW);

			float x, y;

			x = newHexagon.nCol * size;
			y = newHexagon.nRow * size / 2;

			if(newHexagon.nRow % 2 == 0)
			{
				x += (size / 2);
			}

			newHexagon.transform.position = new Vector3(x, 0, y);

			nHexagons++;
		}

	}

	private static void CreateTrio(
		Transform root, Hexagon [] prefabs,
		Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>> stillPossible, 
		Hexagon[,] matrix, int maxCol, int maxRow)
	{
		Hexagon[] trio = new Hexagon[3]; 
		for (int i = 0; i < 3; i++)
		{
			trio[i] = CreateRandomZonePrefab(root, prefabs);
		}

		trio[0].nCol = maxCol / 2;
		trio[0].nRow = maxRow / 2;
		matrix[trio[0].nCol, trio[0].nRow] = trio[0];
		PlaceRelativeTo(trio[1], trio[0], Hexagon.NeighbourgPlace.TOP_LEFT, stillPossible, matrix, maxCol, maxRow);
		PlaceRelativeTo(trio[2], trio[0], Hexagon.NeighbourgPlace.BOT_RIGHT, stillPossible, matrix, maxCol, maxRow);
	}

	private static void TransformPlace(ref int nCol, ref int nRow, Hexagon.NeighbourgPlace place)
	{
		switch (place)
		{
			case Hexagon.NeighbourgPlace.BOT:
				nRow-=2;
				break;
			case Hexagon.NeighbourgPlace.BOT_LEFT:
				nRow--;
				nCol--;
				break;
			case Hexagon.NeighbourgPlace.BOT_RIGHT:
				nRow--;
				nCol++;
				break;
			case Hexagon.NeighbourgPlace.TOP:
				nRow+=2;
				break;
			case Hexagon.NeighbourgPlace.TOP_LEFT:
				nRow++;
				nCol--;
				break;
			case Hexagon.NeighbourgPlace.TOP_RIGHT:
				nRow++;
				nCol++;
				break;
		}
	}

	private static Hexagon.NeighbourgPlace Inverse(Hexagon.NeighbourgPlace place)
	{
		switch (place)
		{
			case Hexagon.NeighbourgPlace.BOT:
				return Hexagon.NeighbourgPlace.TOP;
			case Hexagon.NeighbourgPlace.BOT_LEFT:
				return Hexagon.NeighbourgPlace.TOP_RIGHT;
			case Hexagon.NeighbourgPlace.BOT_RIGHT:
				return Hexagon.NeighbourgPlace.TOP_LEFT;
			case Hexagon.NeighbourgPlace.TOP:
				return Hexagon.NeighbourgPlace.BOT;
			case Hexagon.NeighbourgPlace.TOP_LEFT:
				return Hexagon.NeighbourgPlace.BOT_RIGHT;
			case Hexagon.NeighbourgPlace.TOP_RIGHT:
				return Hexagon.NeighbourgPlace.BOT_LEFT;
		}

		return Hexagon.NeighbourgPlace._NB; // ?
	}

	private static int CountNeighbourgs(int nCol, int nRow, Hexagon[,] matrix, int maxCol, int maxRow)
	{
		int count = 0;

		for (Hexagon.NeighbourgPlace p = (Hexagon.NeighbourgPlace)0; p < Hexagon.NeighbourgPlace._NB; p++)
		{
			int newCol = nCol;
			int newRow = nRow;

			TransformPlace(ref newCol, ref newRow, p);

			if (newCol >= 0 && newRow >= 0 && newCol < maxCol && newRow < maxRow)
			{
				if(matrix[newCol, newRow] != null)
					count++;
			}
		}

		return count;
	}

	private static List<Hexagon.NeighbourgPlace> GetPossiblePlaces(Hexagon hexagon, Hexagon[,] matrix, int maxCol, int maxRow)
	{
		List<Hexagon.NeighbourgPlace> possiblePlaces = new List<Hexagon.NeighbourgPlace>();

		for (Hexagon.NeighbourgPlace p = (Hexagon.NeighbourgPlace)0; p < Hexagon.NeighbourgPlace._NB; p++)
		{
			int nCol = hexagon.nCol;
			int nRow = hexagon.nRow;

			TransformPlace(ref nCol, ref nRow, p);

			if (nCol >= 0 && nRow >= 0 && nCol < maxCol && nRow < maxRow)
			{
				if (matrix[nCol, nRow] == null)
				{
					int count = CountNeighbourgs(nCol, nRow, matrix, maxCol, maxRow);
					if (count >= 2 && count < 6) // Au moins une place libre !
					{
						possiblePlaces.Add(p);
					}
				}
			}
		}

		return possiblePlaces;
	}

	private static bool TakeRandomHexagonAndPlace(
		Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>> stillPossible,
		out Hexagon hexagon, out Hexagon.NeighbourgPlace place)
	{
		hexagon = null;
		place = Hexagon.NeighbourgPlace._NB;

		if (stillPossible.Count == 0)
			return false;

		int nHex = Random.Range(0, stillPossible.Count);
		hexagon = new List<Hexagon>(stillPossible.Keys)[nHex];

		List<Hexagon.NeighbourgPlace> possiblePlaces = stillPossible[hexagon];
		Assert.Check(possiblePlaces.Count != 0, "No possible place, but in stillPossible");

		int nPlace = Random.Range(0, possiblePlaces.Count);
		place = possiblePlaces[nPlace];
		return true;
	}

	private static void SetNeighbourgs(
		Hexagon hexagon, 
		Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>> stillPossible, 
		Hexagon[,] matrix, int maxCol, int maxRow)
	{
		List<Hexagon.NeighbourgPlace> possiblePlaces;

		for (Hexagon.NeighbourgPlace p = (Hexagon.NeighbourgPlace)0; p < Hexagon.NeighbourgPlace._NB; p++)
		{
			int nCol = hexagon.nCol;
			int nRow = hexagon.nRow;

			TransformPlace(ref nCol, ref nRow, p);

			if (nCol >= 0 && nRow >= 0 && nCol < maxCol && nRow < maxRow)
			{
				Hexagon neighbourg = matrix[nCol, nRow];
				if (neighbourg != null)
				{
					hexagon.neighbourgs[(int)p] = neighbourg;
					neighbourg.neighbourgs[(int)Inverse(p)] = hexagon;

					possiblePlaces = GetPossiblePlaces(neighbourg, matrix, maxCol, maxRow);

					if (possiblePlaces.Count == 0)
						stillPossible.Remove(neighbourg);
					else
						stillPossible[neighbourg] = possiblePlaces;
				}
			}
		}

		possiblePlaces = GetPossiblePlaces(hexagon, matrix, maxCol, maxRow);
		if (possiblePlaces.Count > 0)
			stillPossible.Add(hexagon, possiblePlaces);
	}

	private static bool PlaceRelativeTo(
		Hexagon toPlace, Hexagon relative, 
		Hexagon.NeighbourgPlace place, 
		Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>> stillPossible, 
		Hexagon [,] matrix, int maxCol, int maxRow)
	{
		int nCol = relative.nCol;
		int nRow = relative.nRow;
		TransformPlace(ref nCol, ref nRow, place);

		if (matrix[nCol, nRow] != null)
			return false;

		matrix[nCol, nRow] = toPlace;
		toPlace.nCol = nCol;
		toPlace.nRow = nRow;

		SetNeighbourgs(toPlace, stillPossible, matrix, maxCol, maxRow);
		return true;
	}

	private static Hexagon CreateRandomZonePrefab(Transform root, Hexagon [] prefabs)
	{
		Assert.Check(prefabs.Length > 0, "No prefab");
		int n = Random.Range(0, prefabs.Length);

		Hexagon prefab = prefabs[n];
		Assert.Check(prefab != null, "Prefab is null");

		GameObject instance = GameObject.Instantiate(prefab.gameObject) as GameObject;
		Assert.Check(instance, "Instantiate failed");

		Hexagon hexagon = instance.GetComponent<Hexagon>();
		Assert.Check(hexagon, "Unity bug - Component not found");

		hexagon.transform.parent = root;
		hexagon.neighbourgs = new Hexagon[(int)Hexagon.NeighbourgPlace._NB];

		return hexagon;
	}
}

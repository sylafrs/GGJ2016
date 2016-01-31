using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexagonsCreator 
{
	private Transform root;
	private Hexagon [] prefabs;
    private Vector2 hexagonSize;

	private int maxCol, maxRow;
	private Hexagon[,] matrix;

	private bool first;
	private float xMin, yMin, xMax, yMax;

	Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>> stillPossible;

	private HexagonsCreator() {
		
	}

	public static Vector3 CreateZones(Transform root, Hexagon [] prefabs, Vector2 size)
	{
		HexagonsCreator creator = new HexagonsCreator();
		creator.root = root;
		creator.prefabs = prefabs;
        creator.hexagonSize = size;
		creator.first = true;  	

		const int N_HEXAGONS = 21;
		const int MAX_COL = N_HEXAGONS * 2;
		const int MAX_ROW = N_HEXAGONS * 2;

		creator.maxCol = MAX_COL;
		creator.maxRow = MAX_ROW;

		creator.matrix = new Hexagon[MAX_COL, MAX_ROW];
		creator.stillPossible = new Dictionary<Hexagon, List<Hexagon.NeighbourgPlace>>();

		Hexagon [] trio = creator.CreateTrio();
       
        Hexagon current;
		Hexagon.NeighbourgPlace place;

		int nHexagons = 3;
		while (nHexagons < N_HEXAGONS)
		{
			if(!creator.TakeRandomHexagonAndPlace(out current, out place))
				break;
			
			Hexagon newHexagon = creator.CreateRandomZonePrefab();
			creator.PlaceRelativeTo(newHexagon, current, place);
            creator.PlaceHexagon(newHexagon);
			
			nHexagons++;
		}

		Vector3 center = new Vector3 (creator.xMin + (creator.xMax - creator.xMin) / 2, 0, creator.yMin + (creator.yMax - creator.yMin) / 2);

		return center;
	}

    private void PlaceHexagon(Hexagon h)
    {
        float x, y;

        x = h.nCol * this.hexagonSize.x * 1.5f;
        y = h.nRow * this.hexagonSize.y ;

		if (first) {
			xMin = xMax = x;
			yMin = yMax = y;
			first = false;
		} else {
			if (x < xMin)
				xMin = x;
			if (x > xMax)
				xMax = x;
			if (y < yMin)
				yMin = y;
			if (y > yMax)
				yMax = y;
		}

        h.transform.position = new Vector3(x, 0, y);
    }

    private Hexagon[] CreateTrio()
    {
        Hexagon[] trio = new Hexagon[3];
        for (int i = 0; i < 3; i++)
        {
            trio[i] = this.CreateRandomZonePrefab();
        }

        trio[0].nCol = this.maxCol / 2;
        trio[0].nRow = this.maxRow / 2;
        this.matrix[trio[0].nCol, trio[0].nRow] = trio[0];

        this.PlaceRelativeTo(trio[1], trio[0], Hexagon.NeighbourgPlace.TOP_LEFT);
        this.PlaceRelativeTo(trio[2], trio[0], Hexagon.NeighbourgPlace.BOT_RIGHT);

        for (int i = 0; i < 3; i++)
        {
            this.PlaceHexagon(trio[i]);
        }

        return trio;
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

	private int CountNeighbourgs(int nCol, int nRow)
	{
		int count = 0;

		for (Hexagon.NeighbourgPlace p = (Hexagon.NeighbourgPlace)0; p < Hexagon.NeighbourgPlace._NB; p++)
		{
			int newCol = nCol;
			int newRow = nRow;

			TransformPlace(ref newCol, ref newRow, p);

			if (newCol >= 0 && newRow >= 0 && newCol < this.maxCol && newRow < this.maxRow)
			{
				if(this.matrix[newCol, newRow] != null)
					count++;
			}
		}

		return count;
	}

	private List<Hexagon.NeighbourgPlace> GetPossiblePlaces(Hexagon hexagon)
	{
		List<Hexagon.NeighbourgPlace> possiblePlaces = new List<Hexagon.NeighbourgPlace>();

		for (Hexagon.NeighbourgPlace p = (Hexagon.NeighbourgPlace)0; p < Hexagon.NeighbourgPlace._NB; p++)
		{
			int nCol = hexagon.nCol;
			int nRow = hexagon.nRow;

			TransformPlace(ref nCol, ref nRow, p);

			if (nCol >= 0 && nRow >= 0 && nCol < this.maxCol && nRow < this.maxRow)
			{
				if (this.matrix[nCol, nRow] == null)
				{
					int count = this.CountNeighbourgs(nCol, nRow);
					if (count >= 2)
					{
						possiblePlaces.Add(p);
					}
				}
			}
		}

		return possiblePlaces;
	}

	private bool TakeRandomHexagonAndPlace(out Hexagon hexagon, out Hexagon.NeighbourgPlace place)
	{
		hexagon = null;
		place = Hexagon.NeighbourgPlace._NB;

		if (this.stillPossible.Count == 0)
			return false;

		int nHex = Random.Range(0, this.stillPossible.Count);
		hexagon = new List<Hexagon>(this.stillPossible.Keys)[nHex];

		List<Hexagon.NeighbourgPlace> possiblePlaces = this.stillPossible[hexagon];
		Assert.Check(possiblePlaces.Count != 0, "No possible place, but in stillPossible");

		int nPlace = Random.Range(0, possiblePlaces.Count);
		place = possiblePlaces[nPlace];
		return true;
	}

	private void SetNeighbourgs(Hexagon hexagon)
	{
		List<Hexagon.NeighbourgPlace> possiblePlaces;

		for (Hexagon.NeighbourgPlace p = (Hexagon.NeighbourgPlace)0; p < Hexagon.NeighbourgPlace._NB; p++)
		{
			int nCol = hexagon.nCol;
			int nRow = hexagon.nRow;

			TransformPlace(ref nCol, ref nRow, p);

			if (nCol >= 0 && nRow >= 0 && nCol < this.maxCol && nRow < this.maxRow)
			{
				Hexagon neighbourg = matrix[nCol, nRow];
				if (neighbourg != null)
				{
					hexagon.neighbourgs[(int)p] = neighbourg;
					neighbourg.neighbourgs[(int)Inverse(p)] = hexagon;

					possiblePlaces = this.GetPossiblePlaces(neighbourg);

					if (possiblePlaces.Count == 0)
						this.stillPossible.Remove(neighbourg);
					else
						this.stillPossible[neighbourg] = possiblePlaces;
				}
			}
		}

		possiblePlaces = this.GetPossiblePlaces(hexagon);
		if (possiblePlaces.Count > 0)
			this.stillPossible.Add(hexagon, possiblePlaces);
	}

	private bool PlaceRelativeTo(Hexagon toPlace, Hexagon relative, Hexagon.NeighbourgPlace place)
	{
		int nCol = relative.nCol;
		int nRow = relative.nRow;
		TransformPlace(ref nCol, ref nRow, place);

		if (this.matrix[nCol, nRow] != null)
			return false;

		this.matrix[nCol, nRow] = toPlace;
		toPlace.nCol = nCol;
		toPlace.nRow = nRow;

		this.SetNeighbourgs(toPlace);
		return true;
	}

	private Hexagon CreateRandomZonePrefab()
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

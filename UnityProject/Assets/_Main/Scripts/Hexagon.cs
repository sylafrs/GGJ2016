using UnityEngine;
using System.Collections;

public class Hexagon : MonoBehaviour 
{
	public enum NeighbourgPlace
	{
		TOP,
		TOP_LEFT,
		TOP_RIGHT,
		BOT_LEFT,
		BOT_RIGHT,
		BOT,
		_NB
	}

	public Hexagon [] neighbourgs;
	public int nCol;
	public int nRow;
}

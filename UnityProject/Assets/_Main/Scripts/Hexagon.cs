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

	[HideInInspector]
	public Hexagon [] neighbourgs;
	[HideInInspector]
	public int nCol;
	[HideInInspector]
	public int nRow;
}

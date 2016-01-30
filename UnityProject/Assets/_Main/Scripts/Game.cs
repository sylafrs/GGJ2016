using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	public static Game Instance { get; private set; }
	public Player[] Players { get; private set; }
	public List<Zone> Zones { get; private set; }


}

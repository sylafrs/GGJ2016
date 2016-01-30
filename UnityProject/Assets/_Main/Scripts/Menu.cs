using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using System.Collections.Generic;

public class Menu : MonoBehaviour
{
	Dictionary<XboxController, Player> Players = new Dictionary<XboxController, Player>();

	void Start()
	{
		for (int i = 1; i < 5; i++)
			Players.Add ((XboxController)i, null);
	}

	void Update()
	{
		for (int i = 1; i < 5; i++)
		{
			if (Players [(XboxController)i] == null) {
				if (XCI.GetButton (XboxButton.A, (XboxController)i)) {
					Players [(XboxController)i] = Player.Create((XboxController)i);
				}
			} else {
				if (XCI.GetButton (XboxButton.B, (XboxController)i))
				{
					GameObject.Destroy (Players [(XboxController)i].gameObject);
					Players [(XboxController)i] = null;
				}

				if (XCI.GetButton (XboxButton.Start, (XboxController)i)) {
					StartGame ();
				}
			}
		}
	}

	void StartGame()
	{
		Debug.Log ("starting game"); 	
		List<Player> players = new List<Player> ();
		foreach (Player p in Players.Values)
			if (p != null) {
				players.Add (p);
				GameObject.DontDestroyOnLoad (p.gameObject);
			}
		Game.Instance.StartGame (players);
	}
}

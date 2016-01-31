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

	bool gameStarted;
	void Update()
	{
		for (int i = 1; i < 5; i++)
		{
			if (Players [(XboxController)i] == null) {
				if (XCI.GetButtonDown (XboxButton.A, (XboxController)i)) {
					Players [(XboxController)i] = Player.Create((XboxController)i);
				}
			} else {
				if (XCI.GetButtonDown (XboxButton.B, (XboxController)i))
				{
					GameObject.Destroy (Players [(XboxController)i].gameObject);
					Players [(XboxController)i] = null;
				}

				if (XCI.GetButtonDown (XboxButton.Start, (XboxController)i)) {
					if (!gameStarted)
					{
						gameStarted = true;
						StartGame();
					}
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

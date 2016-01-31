using UnityEngine;
using System.Collections;
using XboxCtrlrInput;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
	public Image[] Join;
	public Image[] Ready;

	public Text nbrRound;
	public Text time;

	public UnityEvent OnGameStart;

	bool WaitingPlayers;
	bool inGame;

	Dictionary<XboxController, Player> Players = new Dictionary<XboxController, Player>();

	void Start()
	{
		for (int i = 1; i < 5; i++)
			Players.Add ((XboxController)i, null);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void StartWaitingForPlayers()
	{
		for (int i = 1; i < 5; i++)
		{
			if (Players [(XboxController)i] != null)
			{
				GameObject.Destroy (Players [(XboxController)i].gameObject);
				Players [(XboxController)i] = null;
			}

			Join[i - 1].enabled = true;
			Ready[i - 1].enabled = false;	
		}

		WaitingPlayers = true;
	}

	public void StopWaiting()
	{
		WaitingPlayers = false;
	}

	bool gameStarted;
	void Update()
	{
		if (!WaitingPlayers) {
			for (int i = 1; i < 5; i++) {
				if (Players [(XboxController)i] == null) {
					if (XCI.GetButtonDown (XboxButton.A, (XboxController)i)) {
						Player p = Player.Create ((XboxController)i);
						p.gameObject.SetActive (false);
						Players [(XboxController)i] = p;

						Join [i - 1].enabled = false;
						Ready [i - 1].enabled = true;
					}
				} else {
					if (XCI.GetButtonDown (XboxButton.B, (XboxController)i)) {
						GameObject.Destroy (Players [(XboxController)i].gameObject);
						Players [(XboxController)i] = null;
						Join [i - 1].enabled = true;
						Ready [i - 1].enabled = false;
					}

					if (XCI.GetButtonDown (XboxButton.Start, (XboxController)i)) {
						if (!gameStarted) {
							gameStarted = true;
							StartGame ();
						}
					}
				}
			}
		}
		else if (inGame) {
			nbrRound.text = Game.Instance.nbrRound.ToString();
		}
	}

	void StartGame()
	{
		Debug.Log ("starting game"); 	
		List<Player> players = new List<Player> ();
		foreach (Player p in Players.Values)
			if (p != null) {
				p.gameObject.SetActive(true);
				players.Add (p);
				GameObject.DontDestroyOnLoad (p.gameObject);
			}
		Game.Instance.StartGame (players);
		OnGameStart.Invoke();
		StopWaiting();
	}
}

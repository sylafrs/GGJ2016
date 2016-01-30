using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public static Game Instance { get; private set; }
	public Player[] Players { get; private set; }
	public List<Zone> Zones { get; private set; }

	public GameSettings Settings; 

	void Awake()
	{
		Assert.Check (Settings, "GameSettings not setted");
		Assert.Check (Instance == null, "Instance already setted");
		Instance = this;
		GameObject.DontDestroyOnLoad (this.gameObject);
	}
		
	public void StartGame(List<Player> listPlayer)
	{
		Assert.Check (listPlayer == null || listPlayer.Count < 2, "Not enough players");
		this.Players = listPlayer.ToArray ();
		StartCoroutine (RunGame ());
	}

	public IEnumerator RunGame()
	{
		yield return SceneManager.LoadSceneAsync (1, LoadSceneMode.Single);

		//Players = GameObject.FindObjectsOfType<Player>();
		Zones 	= new List<Zone>(GameObject.FindObjectsOfType<Zone> ());

		foreach (Zone z in Zones)
		{
			z.PlaceUI (Settings.ZoneUIPrefab);
		}

		foreach (Player p in Players)
			p.rigidbody.isKinematic = false;

		while (true)
		{
			yield return new WaitForEndOfFrame ();

			foreach (Zone z in Zones)
				z.GameUpdate ();

			foreach (Player p in Players)
				p.GameUpdate ();
		}
	}
}

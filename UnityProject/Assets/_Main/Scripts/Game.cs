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

	void Start()
	{
		if(SceneManager.GetActiveScene().buildIndex == 1)
		{
			List<Player> players = new List<Player> ();
			players.Add(Player.Create (XboxCtrlrInput.XboxController.First));
			players.Add(Player.Create (XboxCtrlrInput.XboxController.Second));

			StartGame (players);
		}
	}
		
	public void StartGame(List<Player> listPlayer)
	{
		Assert.Check (listPlayer != null, "Player list is null");
		Assert.Check (listPlayer.Count >= 2, "Not enough players (" + listPlayer.Count + ")");
		this.Players = listPlayer.ToArray ();
		StartCoroutine (RunGame ());
	}

	public IEnumerator RunGame()
	{
		if(SceneManager.GetActiveScene().buildIndex != 1)
			yield return SceneManager.LoadSceneAsync (1, LoadSceneMode.Single);

		//Players = GameObject.FindObjectsOfType<Player>();
		Zones 	= new List<Zone>(GameObject.FindObjectsOfType<Zone> ());

		Assert.Check (Settings.Effects.Length != 0, "No effect setted");
		foreach (Zone z in Zones)
		{
			z.Effect = Settings.Effects [Random.Range (0, Settings.Effects.Length)];

			for (int i = 0; i < z.Effect.NeededLetters; i++)
				z.AddKeyToCombinaison ((KeyCode)(Random.Range ((int)'A', (int)'Z' + 1) - (int)'A' + (int)KeyCode.A));

			z.PlaceUI (Settings.ZoneUIPrefab, Settings.LetterPrefab);
		}

		foreach (Player p in Players)
			p.rigidbody.isKinematic = false;

		while (true)
		{
			yield return null;

			foreach (Zone z in Zones)
				z.GameUpdate ();

			foreach (Player p in Players)
				p.GameUpdate ();
		}
	}
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

using Random=UnityEngine.Random;

public class Game : MonoBehaviour {

	public static Game Instance { get; private set; }
	public Player[] Players { get; private set; }
	public List<Zone> Zones { get; private set; }

	private float actualDuractionGame;

	public GameSettings Settings; 

	void Awake()
	{
		Assert.Check (Settings, "GameSettings not setted");
		Assert.Check (Instance == null, "Instance already setted");
		Instance = this;
		GameObject.DontDestroyOnLoad (this.gameObject);

		actualDuractionGame = Settings.DurationGame;
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
		int i;

		if(SceneManager.GetActiveScene().buildIndex != 1)
			yield return SceneManager.LoadSceneAsync (1, LoadSceneMode.Single);

		//Players = GameObject.FindObjectsOfType<Player>();
		Zones 	= new List<Zone>(GameObject.FindObjectsOfType<Zone> ());

		Assert.Check (Settings.Effects.Length != 0, "No effect setted");

		List<ZoneEffect> effects = new List<ZoneEffect> (Settings.Effects);
		effects.Remove (Settings.Effects [0]);

//		foreach (Zone z in Zones)
//		{
//			if (effects.Count == 0)
//			{
//				z.Effect = Settings.Effects [0]; // Default effect.
//			}
//			else
//			{
//				int nEffect = Random.Range (-1, effects.Count);
//				if (nEffect == -1)
//				{
//					z.Effect = Settings.Effects [0]; // Default effect.
//				}
//				else
//				{
//					z.Effect = effects [nEffect];
//					effects.Remove (z.Effect);
//				}
//			}
//
//			for (i = 0; i < z.Effect.NeededLetters; i++)
//				z.AddKeyToCombinaison ((KeyCode)(Random.Range ((int)'A', (int)'Z' + 1) - (int)'A' + (int)KeyCode.A));
//
//			z.PlaceUI (Settings.ZoneUIPrefab, Settings.LetterPrefab);
//		}

		foreach (ZoneEffect z in Settings.Effects)
		{
			int nZones = Random.Range (0, Zones.Count);
			Zones [nZones].Effect = z;
		}

		foreach (Zone z in Zones)
		{
			if(z.Effect == null)
				z.Effect = Settings.Effects [0];

			for (i = 0; i < z.Effect.NeededLetters; i++)
				z.AddKeyToCombinaison ((KeyCode)(Random.Range ((int)'A', (int)'Z' + 1) - (int)'A' + (int)KeyCode.A));
			
			z.PlaceUI (Settings.ZoneUIPrefab, Settings.LetterPrefab);
		}

		i = 0;
		foreach (Player p in Players)
		{
			p.color = Settings.PlayerColors [i];
			p.rigidbody.isKinematic = false;
			i++;
		}

		DateTime start = DateTime.Now;

		while (actualDuractionGame <= Settings.DurationGame)
		{
			yield return null;

			DateTime now = DateTime.Now;
			actualDuractionGame = (float)(now - start).TotalSeconds;

			foreach (Zone z in Zones)
				z.GameUpdate ();

			foreach (Player p in Players)
				p.GameUpdate ();
		}
	}
}

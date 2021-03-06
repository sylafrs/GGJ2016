﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

using Random=UnityEngine.Random;
using UnityEngine.Events;

public class Game : MonoBehaviour {

	public static Game Instance { get; private set; }
	public Player[] Players { get; private set; }
	public List<Zone> Zones { get; private set; }

	public AudioSource musicBackground;

	public int nbrRound = 0;

	public static bool paused;
	private float actualDuractionGame;

	public GameSettings Settings;

	public UnityEvent OnRoundStarted, OnRoundFinished;
	public UnityEvent OnGameFinished;

	void Awake()
	{
		Assert.Check (Settings, "GameSettings not setted");
		Assert.Check (Instance == null, "Instance already setted");
		Instance = this;
		GameObject.DontDestroyOnLoad (this.gameObject);

		actualDuractionGame = Settings.DurationGame;
	}


	string gui_remainingTime = "";
	void OnGUI()
	{
		GUILayout.Label ("Remaining time = " + gui_remainingTime);
	}
		
	public void StartGame(List<Player> listPlayer)
	{
		Assert.Check (listPlayer != null, "Player list is null");
		Assert.Check (listPlayer.Count >= 2, "Not enough players (" + listPlayer.Count + ")");
		this.Players = listPlayer.ToArray ();
		StartCoroutine (RunGame ());
	}
		
	private bool finished;

	private IEnumerator RunGame()
	{
		finished = false;
		while (!finished)
		{
			bool mustPause = false;
			DateTime start = DateTime.Now;
			actualDuractionGame = 0;

			DateTime lastPause;
			float pauseTime = 0;
			float timeWhilePause = 0;
			paused = false;
			Time.timeScale = 1; 

			OnRoundStarted.Invoke();
			InitGame();

			while (actualDuractionGame <= Settings.DurationGame)
			{
				yield return null;

				DateTime now = DateTime.Now;

				foreach (Zone z in Zones)
					if(!paused)
						z.GameUpdate();

				mustPause = false;
				foreach (Player p in Players)
				{
					if(!paused)
						p.GameUpdate();
					
					if (p.AskPause)
					{
						mustPause = true;
					}
				}

				if (mustPause) {
					paused = !paused;
					if (paused)
						lastPause = now;
					else
						timeWhilePause += pauseTime;
					Time.timeScale = paused ? 0 : 1;
				}

				if (paused)
				{
					pauseTime = (float)(now - lastPause).TotalSeconds;
				}
				else
				{
					actualDuractionGame = (float)(now - start).TotalSeconds - timeWhilePause;
				}

				gui_remainingTime = (Settings.DurationGame - actualDuractionGame).ToString("F2");
			}

			EndGame();

			OnRoundFinished.Invoke();

			inScoreMenu = true;
			while (inScoreMenu)
				yield return null;
		}

		OnGameFinished.Invoke();
	}

	public bool inScoreMenu;

	public void ContinueGame()
	{
		inScoreMenu = false;
	}

	private void InitZoneEffects()
	{
		Assert.Check (Settings.Effects.Length != 0, "No effect setted");

		List<Zone> zonesToSet = new List<Zone> (this.Zones);
		for (int i = 1; i < Settings.Effects.Length && zonesToSet.Count > 0; i++)
		{
			int nZones = Random.Range (0, zonesToSet.Count);
			zonesToSet [nZones].Effect = Settings.Effects [i];
			zonesToSet.Remove (zonesToSet [nZones]);
		}

		foreach (Zone z in zonesToSet) 
		{
			z.Effect = Settings.Effects [0];
		}
	}

	private void InitZoneCombinaisons()
	{
		foreach (Zone z in Zones) 
		{
			for (int i = 0; i < z.Effect.NeededLetters; i++)
				z.AddKeyToCombinaison ((KeyCode)(Random.Range ((int)'A', (int)'Z' + 1) - (int)'A' + (int)KeyCode.A));

			z.PlaceUI (Settings.ZoneUIPrefab, Settings.LetterPrefab);
		}
	}

	private void InitPlayers()
	{
		int i = 0;
		List<Zone> availableZones = new List<Zone> (Zones);
		foreach (Player p in Players) 
		{
			p.colorZones = Settings.ZonesColors [i];
			p.rigidbody.isKinematic = false;

			Zone z = availableZones[Random.Range (0, availableZones.Count)];
			availableZones.Remove (z);

			p.transform.position = z.transform.position;
			i++;
		}
	}

	private void InitGame()
	{
		this.transform.position = Vector3.zero;
		this.transform.Translate(-HexagonsCreator.CreateZones(this.transform, Settings.ZonePrefabs, new Vector2(5, 4.3f)));

		//Players = GameObject.FindObjectsOfType<Player>();
		Zones = new List<Zone> (GameObject.FindObjectsOfType<Zone> ());

		this.InitZoneEffects ();
		this.InitZoneCombinaisons ();
		this.InitPlayers ();

		//launch game
	}

	public void RemoveZone(Zone z)
	{
		if (Zones.Contains(z))
		{
			Zones.Remove(z);
			if(z.Owner)
				z.Owner.OnZoneLost(z);
			z.CleanUp();
			GameObject.Destroy(z.gameObject);
		}
	}

	private void EndGame()
	{
		//SON Time's up

		nbrRound++;

		Player best = Players [0];
		for (int i = 1; i < Players.Length; i++)
			if (Players [i].OwnedZones.Count > best.OwnedZones.Count)
				best = Players [i];

		foreach (Player p in Players)
			if (p.OwnedZones.Count == best.OwnedZones.Count) {
				p.nbrRoundWin++;
				if (p.nbrRoundWin == 3)
					finished = true;
			}
		
		foreach (Player p in Players)
		{
			p.Clear();
		}

		foreach (Zone z in Zones)
		{
			z.CleanUp();
			GameObject.Destroy(z.gameObject);
		}

		Zones = null;

	}
}

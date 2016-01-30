using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public Color color;
	public float speed;

	public List<Zone> OwnedZones { get; private set; }
	public List<ZoneEffect> ActiveEffects { get; private set; }
	public Zone Position { get; private set; }

	public Player() : base() 
	{
		// Don't call Unity functions here.
		OwnedZones		= new List<Zone> ();
		ActiveEffects 	= new List<ZoneEffect> ();
	}

	public void OnEnterZone(Zone z)
	{
		Position = z;

		#warning Temporary : take over when visiting.
		if(z.CanBeTakenOver)
			z.OnPlayerTakeOver(this);
	}

	public void OnLeaveZone(Zone z) 
	{
		Position = null;
	}

	#warning TODO : ReadInput.
	private PlayerInput ReadInput() 
	{
		return default(PlayerInput);
	}

	#warning TODO : UpdatePlayer
	private void UpdatePlayer(PlayerInput input) 
	{
		
	}

	public void GameUpdate()
	{
		PlayerInput input = ReadInput ();

		foreach (ZoneEffect z in ActiveEffects)
			z.ApplyInputBuff (ref input);

		this.UpdatePlayer (input);
	}
}

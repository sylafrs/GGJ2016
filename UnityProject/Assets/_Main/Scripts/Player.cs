using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;

[RequireComponent (typeof (Rigidbody))]

public class Player : MonoBehaviour
{
	public Color color;
	public float speed;

	public XboxController controller;

	private Rigidbody rigidbody;

	public List<Zone> OwnedZones { get; private set; }
	public List<ZoneEffect> ActiveEffects { get; private set; }
	public Zone Position { get; private set; }

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();
	}

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

	private PlayerInput ReadInput() 
	{
		PlayerInput input;

		input.moveAxis.x = XCI.GetAxis(XboxAxis.LeftStickX, controller);
		input.moveAxis.y = XCI.GetAxis(XboxAxis.LeftStickY, controller);
		input.validateZoneButtonPressed = XCI.GetButtonDown (XboxButton.LeftBumper, controller);

		if(XCI.GetAxis(XboxAxis.LeftTrigger, controller) != 0)
			input.fireButtonPressed = true;
		else
			input.fireButtonPressed = false;
		return input;
	}

	#warning TODO : UpdatePlayer
	private void UpdatePlayer(PlayerInput input) 
	{
		Vector3 forceRigid = new Vector3 (input.moveAxis.x, 0, input.moveAxis.y);

		rigidbody.AddForce (forceRigid * speed, ForceMode.VelocityChange);
	}

	public void GameUpdate()
	{
		PlayerInput input = ReadInput ();

		foreach (ZoneEffect z in ActiveEffects)
			z.ApplyInputBuff (ref input);

		this.UpdatePlayer (input);
	}
}

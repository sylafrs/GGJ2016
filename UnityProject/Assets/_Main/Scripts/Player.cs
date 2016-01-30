using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;

[RequireComponent (typeof (Rigidbody))]

public class Player : MonoBehaviour
{
	public Color color;
	public float speed;

	public GameObject Bullet;

	public bool isGrounded;

	public XboxController controller;

	public LayerMask LayerMask;

	public new Rigidbody rigidbody { get; private set; }

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

	public static Player Create(XboxController i)
	{
		Player player = (GameObject.Instantiate(Game.Instance.Settings.PlayerPrefab.gameObject) as GameObject).GetComponent<Player>();
		player.rigidbody.isKinematic = true;
		player.controller = i;
		return player;
	}

	public void OnEnterZone(Zone z)
	{
		Position = z;
	}

	public void OnLeaveZone(Zone z) 
	{
		//Position = null;
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

		GameObject bulletReference = null;

		rigidbody.AddForce (forceRigid * speed, ForceMode.VelocityChange);

		Quaternion targetRotation = Quaternion.LookRotation(forceRigid, Vector3.up);

		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, 15.0f * Time.deltaTime);

		rigidbody.MoveRotation(newRotation);

		RaycastHit hit;

		if (Physics.Raycast (transform.position, Vector3.down, 100, LayerMask))
		{
			isGrounded = true;
		}
		else
			isGrounded = false;

		if (input.validateZoneButtonPressed)
		{
			if (Position && Position.CanBeTakenOver)
				Position.OnPlayerTakeOver (this);
		}
		if (input.fireButtonPressed)
		{
			bulletReference = Instantiate(Bullet, transform.position, Quaternion.identity) as GameObject;
		}
	}

	public void GameUpdate()
	{
		PlayerInput input = ReadInput ();

		foreach (ZoneEffect z in ActiveEffects)
			z.ApplyInputBuff (ref input);

		this.UpdatePlayer (input);
	}
}

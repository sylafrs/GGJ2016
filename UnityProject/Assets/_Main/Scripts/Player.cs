using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;
using UnityEngine.Events;

[RequireComponent (typeof (Rigidbody))]

public class Player : MonoBehaviour
{
	const float MAX_DISTANCE_GROUND = 1;

	//vitesse de la boule float

	public Color 			colorZones;
	public float 			speed;
	public float 			multiplicatorSpeedBullet = 1;
	public int 				nbrRoundWin = 0;
	public GameObject 		Bullet;
	public bool 			rightBullet = true;
	public bool 			isGrounded;

	public XboxController 	controller;

	public LayerMask 		LayerMask;

	private Quaternion 		targetRotation = Quaternion.identity;

	public new Rigidbody 	rigidbody 		{ get; private set; }


	public Zone 			LastOwnedZone 	{ get; private set; }
	public List<Zone> 		OwnedZones 		{ get; private set; }
	public List<ZoneEffect> ActiveEffects 	{ get; private set; }
	public Zone 			Position 		{ get; private set; }

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
		player.GetComponentInChildren<MeshRenderer> ().material.color = Game.Instance.Settings.PlayerColors [(int)i];
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

	public bool AskRestart {
		get {
			return XCI.GetButtonDown(XboxButton.Start, controller);
		}
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

	IEnumerator WaitFire(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		rightBullet = true;
	}

	private void UpdatePlayer(PlayerInput input) 
	{
		GameObject bulletReference = null;

		if (Physics.Raycast (transform.position, Vector3.down, MAX_DISTANCE_GROUND, LayerMask))
		{
			isGrounded = true;
		}
		else
			isGrounded = false;

		if (!isGrounded)
			return;

		Vector3 forceRigid = new Vector3 (input.moveAxis.x, 0, input.moveAxis.y);
		if (forceRigid != Vector3.zero)
		{
			targetRotation = Quaternion.LookRotation (forceRigid, Vector3.up);
			rigidbody.AddForce (forceRigid * speed, ForceMode.VelocityChange);
		}

		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, 5.0f * Time.deltaTime);
		rigidbody.MoveRotation(newRotation);

		if (input.validateZoneButtonPressed)
		{
			if (Position && Position.CanBeTakenOver && Position.Owner != this)
			{
				//SON Prise controlle zone
				if (Position.Owner != null)
					Position.Owner.OnZoneLost (Position);
				this.OnZoneWon (Position);
			}
		}

		if (input.fireButtonPressed && rightBullet)
		{
			//SON Tire
			bulletReference = Instantiate(Bullet, transform.position, transform.rotation) as GameObject;
			bulletReference.GetComponent<MoveBullet> ().playerOwner = this.gameObject;
			bulletReference.GetComponent<MoveBullet> ().multiSpeedBullet = this.multiplicatorSpeedBullet;
			rightBullet = false;
			StartCoroutine (WaitFire (0.2f));
		}
	}

	public void DetectBullet(Vector3 direction)
	{
		rigidbody.AddForce (direction * 10 * multiplicatorSpeedBullet, ForceMode.VelocityChange);
	}

	private void OnZoneWon(Zone zone)
	{
		LastOwnedZone = zone;
		zone.OnPlayerTakeOver (this);
		OwnedZones.Add (zone);
	}

	private void OnZoneLost(Zone zone)
	{
		OwnedZones.Remove (zone);
	}

	private void UpdateZone()
	{
		Zone near = Game.Instance.Zones[0];
		float sqrDistanceNear = (this.transform.position - near.transform.position).sqrMagnitude;

		for (int i = 1; i < Game.Instance.Zones.Count; i++) {

			Zone zone = Game.Instance.Zones [i];
			float sqrDistance = (this.transform.position - zone.transform.position).sqrMagnitude;

			if (sqrDistance < sqrDistanceNear) {
				sqrDistanceNear = sqrDistance;
				near = zone;
			}
		}

		if (this.Position != near) {
			if (this.Position != null) {
				this.Position.OnPlayerLeave (this);
				this.OnLeaveZone (this.Position);
			}

			near.OnPlayerEnter (this);
			this.OnEnterZone (near);
		}
	}

	public void AddBuff(ZoneEffect effect)
	{
		ActiveEffects.Add (effect);
	}

	public void RemoveBuff(ZoneEffect effect)
	{
		ActiveEffects.Remove (effect);
	}

	public YieldInstruction WaitUntilLastZoneModified()
	{
		return Game.Instance.StartCoroutine(WaitUntilLastZoneModifiedEnum());
	}

	private IEnumerator WaitUntilLastZoneModifiedEnum()
	{
		Zone zone = LastOwnedZone;
		while(LastOwnedZone == zone && LastOwnedZone.Owner == this)
		{
			yield return null;
		}
	}

	public void GameUpdate()
	{
		this.UpdateZone ();

		PlayerInput input = ReadInput ();

		foreach (ZoneEffect z in ActiveEffects)
			z.ApplyInputBuff (ref input);

		this.UpdatePlayer (input);
	}
		
	public void Clear()
	{
		targetRotation = Quaternion.identity;
		LastOwnedZone = null;
		OwnedZones.Clear();
		ActiveEffects.Clear();
		Position = null;
	}
}

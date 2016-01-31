using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;
using UnityEngine.Events;

[RequireComponent (typeof (Rigidbody))]

public class Player : MonoBehaviour
{
	const float MAX_DISTANCE_GROUND = 10;

	//vitesse de la boule float

	public Color 			colorZones;
	public float 			speed;
	public float 			multiplicatorSpeedBullet = 1;
	public int 				nbrRoundWin = 0;
	public GameObject 		Bullet;
	public bool 			rightBullet = true;
	public bool 			isGrounded;
	public Vector3?			WindForce;

	public XboxController 	controller;

	public LayerMask 		LayerMask;

	private Quaternion 		targetRotation = Quaternion.identity;

	public new Rigidbody 	rigidbody 		{ get; private set; }
	public new Animator 	animator 		{ get; private set; }


	public Zone 			LastOwnedZone 	{ get; private set; }
	public List<Zone> 		OwnedZones 		{ get; private set; }
	public List<ZoneEffect> ActiveEffects 	{ get; private set; }
	public Zone 			Position 		{ get; private set; }

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();
		animator = GetComponentInChildren<Animator> ();
	}

	public Player() : base() 
	{
		// Don't call Unity functions here.
		OwnedZones		= new List<Zone> ();
		ActiveEffects 	= new List<ZoneEffect> ();
	}

	public static Player Create(XboxController i)
	{

		Player player = (GameObject.Instantiate(Game.Instance.Settings.PlayerPrefabs[(int)i - 1].gameObject) as GameObject).GetComponent<Player>();
		player.rigidbody.isKinematic = true;
		MeshRenderer [] renderers = player.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer r in renderers)
			r.material.color = Game.Instance.Settings.PlayerColors [(int)i - 1];
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

	public bool AskPause {
		get
		{
			return XCI.GetButtonDown(XboxButton.Start, controller);
		}
	}

	public void Vibrate(float leftMotor, float rightMotor)
	{
		XInputDotNetPure.GamePad.SetVibration((XInputDotNetPure.PlayerIndex)controller, leftMotor, rightMotor);
	}

	public void OnDisable()
	{
		Vibrate(0, 0);
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

		isGrounded = Physics.Raycast (transform.position + Vector3.up, Vector3.down, MAX_DISTANCE_GROUND, LayerMask);
		if (!isGrounded)
			return;

		Vector3 forceRigid = new Vector3 (input.moveAxis.x, 0, input.moveAxis.y);
		if (forceRigid != Vector3.zero && rightBullet)
		{
			targetRotation = Quaternion.LookRotation (forceRigid, Vector3.up);
			rigidbody.AddForce (forceRigid * speed, ForceMode.Force);

			animator.SetFloat ("Move", 1);
		}
		else
			animator.SetFloat ("Move", 0);

		if(WindForce.HasValue)
			rigidbody.AddForce(WindForce.Value, ForceMode.Force);

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
			animator.SetTrigger("Attack");

			bulletReference = Instantiate(Bullet, transform.position + new Vector3(0, 2, 0), transform.rotation) as GameObject;
			bulletReference.GetComponent<MoveBullet> ().playerOwner = this.gameObject;
			bulletReference.GetComponent<MoveBullet> ().multiSpeedBullet = this.multiplicatorSpeedBullet;

			rightBullet = false;
			StartCoroutine (WaitFire (1.0f));
		}
	}

	public void DetectBullet(Vector3 direction)
	{
		animator.SetTrigger("Hit");
		rigidbody.AddForce (direction * 20 * multiplicatorSpeedBullet, ForceMode.VelocityChange);
	}

	public void OnZoneWon(Zone zone)
	{
		LastOwnedZone = zone;
		zone.OnPlayerTakeOver (this);
		OwnedZones.Add (zone);
	}

	public void OnZoneLost(Zone zone)
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
		while(this && LastOwnedZone && LastOwnedZone == zone && LastOwnedZone.Owner == this)
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

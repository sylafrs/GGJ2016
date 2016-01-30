using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]


public class MoveBullet : MonoBehaviour
{
	public float speed = 15.0f;
	public float multiSpeedBullet = 1;

	public GameObject playerOwner;

	public new Rigidbody rigidbody { get; private set; }

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start ()
	{
		Destroy(gameObject, 1.0f);

		rigidbody.AddForce (transform.forward * speed * multiSpeedBullet, ForceMode.VelocityChange);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject != playerOwner && other.tag == "Player")
			other.gameObject.GetComponent<Player> ().DetectBullet (transform.forward);
	}
}

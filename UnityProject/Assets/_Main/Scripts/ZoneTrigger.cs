using UnityEngine;
using System.Collections;

public class ZoneTrigger : MonoBehaviour {

	private Zone zone;

	public void Awake()
	{
		zone = this.transform.parent.GetComponent<Zone> ();
	}

	public void OnTriggerEnter(Collider c)
	{
		zone.OnTriggerEnter (c);
	}

	public void OnTriggerExit(Collider c)
	{
		zone.OnTriggerExit (c);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerRespawner : MonoBehaviour {

	void OnTriggerEnter(Collider c)
	{
		Player p = c.GetComponent<Player> ();
		if (p)
		{
			if (Game.Instance.Zones == null || Game.Instance.Zones.Count == 0)
				return;

			List<Zone> possibleZones = new List<Zone> ();
			foreach (Zone z in Game.Instance.Zones)
				if (z.VisitorsCount == 0)
					possibleZones.Add (z);

			if (possibleZones.Count == 0)
				p.transform.position = Game.Instance.Zones [0].transform.position; // cas particulier
			else
				p.transform.position = possibleZones [Random.Range (0, possibleZones.Count)].transform.position;
		}
	}
}

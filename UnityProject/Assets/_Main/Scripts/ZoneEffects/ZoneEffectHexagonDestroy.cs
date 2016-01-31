using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class ZoneEffectHexagonDestroy : ZoneEffect {

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		List<Zone> possibleZones = new List<Zone>();

		foreach (Zone z2 in Game.Instance.Zones)
			if (z2.VisitorsCount == 0)
				possibleZones.Add(z2);

		if (possibleZones.Count != 0)
		{
			Zone toRemove = possibleZones[Random.Range(0, possibleZones.Count)];
			Game.Instance.RemoveZone(toRemove);
		}

		yield break;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

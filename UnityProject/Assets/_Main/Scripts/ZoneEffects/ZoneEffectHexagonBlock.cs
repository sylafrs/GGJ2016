using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class ZoneEffectHexagonBlock : ZoneEffect {

	public override IEnumerator ApplyEffect (Player player)
	{
		List<Zone> possibleZones = new List<Zone>();

		foreach (Zone z in Game.Instance.Zones)
			if (z.VisitorsCount == 0 && !z.LockTakeOver)
				possibleZones.Add(z);

		if (possibleZones.Count != 0)
		{
			Zone toBlock = possibleZones[Random.Range(0, possibleZones.Count)];
			toBlock.BlockingRock.SetActive(true);
			toBlock.LockTakeOver = true;
			yield return player.WaitUntilLastZoneModified();
			toBlock.BlockingRock.SetActive(false);
			toBlock.LockTakeOver = false;
		}

	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

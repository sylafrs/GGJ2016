using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class ZoneEffectHexagonBlock : ZoneEffect {
    
    public AudioClip clip;
    
	public override IEnumerator ApplyEffect (Zone z, Player player)
	{

        Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);

        List<Zone> possibleZones = new List<Zone>();

		foreach (Zone z2 in Game.Instance.Zones)
			if (z2.VisitorsCount == 0 && !z2.LockTakeOver)
				possibleZones.Add(z2);

		if (possibleZones.Count != 0)
		{
			Zone toBlock = possibleZones[Random.Range(0, possibleZones.Count)];
			toBlock.BlockingRock.SetActive(true);
			toBlock.LockTakeOver = true;

			z.OnEffectActivated();
			yield return player.WaitUntilLastZoneModified();
			z.OnEffectFinished();

			if (toBlock)
			{
				if(toBlock.BlockingRock)
					toBlock.BlockingRock.SetActive(false);
				toBlock.LockTakeOver = false;
			}
		}

	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectFlipCamera : ZoneEffect 
{
	Player last;

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		last = player;
		CameraFlip flipper_le_dauphin = Camera.main.GetComponent<CameraFlip>();
		//Zone z = player.LastOwnedZone;

		flipper_le_dauphin.inverse = true;
		z.OnEffectActivated();
		yield return player.WaitUntilLastZoneModified();
		if(player == last)
		{
			flipper_le_dauphin.inverse = false;
			z.OnEffectFinished();
		}
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing
	}
}

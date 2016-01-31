using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectLight : ZoneEffect {

	#warning TODO : Zone effect light.

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		z.OnEffectActivated();
		yield return player.WaitUntilLastZoneModified();
		z.OnEffectFinished();
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectLight : ZoneEffect {

	#warning TODO : Zone effect light.

	public override IEnumerator ApplyEffect (Player player)
	{
		
		yield return player.WaitUntilLastZoneModified();

	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

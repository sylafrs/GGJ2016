using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectNothing : ZoneEffect {

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		// Does nothing.
		yield break;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}

}

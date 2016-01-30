using UnityEngine;
using System.Collections;

public class ZoneEffectNothing : ZoneEffect {

	public override void ApplyEffect (Player player)
	{
		// Does nothing.
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}

}

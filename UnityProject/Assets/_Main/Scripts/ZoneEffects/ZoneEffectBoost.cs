using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectBoost : ZoneEffect {

    public float boostMultiplier;

    public override IEnumerator ApplyEffect (Player player)
	{
        player.speed = player.speed * boostMultiplier;

		yield break;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}

}

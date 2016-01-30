using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectBoost : ZoneEffect {

    public float boostMultiplier;

    public override IEnumerator ApplyEffect (Player player)
	{
		player.AddBuff (this);
		yield return player.WaitUntilLastZoneModified();
		player.RemoveBuff (this);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		data.moveAxis *= boostMultiplier;
	}
}

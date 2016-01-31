using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectSpeedBoostOthers : ZoneEffect {

	public float boostMultiplicator;

    public override IEnumerator ApplyEffect (Player player)
	{
		foreach(Player p in Game.Instance.Players)
			if(p != player)
				player.AddBuff (this);

		yield return player.WaitUntilLastZoneModified();

		foreach(Player p in Game.Instance.Players)
			if(p != player)
				player.RemoveBuff (this);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		data.moveAxis *= -1;
	}
}

using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectShootStop : ZoneEffect {

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		foreach(Player p in Game.Instance.Players)
			if(p != player)
				player.AddBuff (this);
		z.OnEffectActivated();
		yield return player.WaitUntilLastZoneModified();
		z.OnEffectFinished();
		foreach(Player p in Game.Instance.Players)
			if(p != player)
				player.RemoveBuff (this);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		data.fireButtonPressed = false;
	}
}

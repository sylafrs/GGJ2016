﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectWindOthers : ZoneEffect {

	public Vector3 wind;

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		foreach (Player p in Game.Instance.Players)
			if (p != player)
				p.WindForce = wind;

		z.OnEffectActivated();

		yield return player.WaitUntilLastZoneModified();

		z.OnEffectFinished();

		foreach (Player p in Game.Instance.Players)
			if (p != player)
				p.WindForce = null;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

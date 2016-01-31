﻿using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectSpeedBoost : ZoneEffect {

    public float boostMultiplier;
    public AudioClip clip;

    public override IEnumerator ApplyEffect (Player player)
	{
        Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);

        player.AddBuff (this);
		yield return player.WaitUntilLastZoneModified();
		player.RemoveBuff (this);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		data.moveAxis *= boostMultiplier;
	}
}

using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectShootBoost : ZoneEffect
{
	public float boostMultiplier;
    public AudioClip sound;

	public override IEnumerator ApplyEffect (Player player)
	{
        Camera.main.GetComponent<AudioSource>().PlayOneShot(sound);

        player.multiplicatorSpeedBullet *= boostMultiplier;	
		yield return player.WaitUntilLastZoneModified();
		player.multiplicatorSpeedBullet /= boostMultiplier;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing
	}
}

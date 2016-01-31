using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectShootBoost : ZoneEffect
{
	public float boostMultiplier;
    public AudioClip sound;

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{

        Camera.main.GetComponent<AudioSource>().PlayOneShot(sound);

        player.multiplicatorSpeedBullet *= boostMultiplier;	

		z.OnEffectActivated();

		yield return player.WaitUntilLastZoneModified();
		z.OnEffectFinished();
		player.multiplicatorSpeedBullet /= boostMultiplier;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing
	}
}

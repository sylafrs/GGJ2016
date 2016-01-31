using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectSpeedBoost : ZoneEffect {

    public float boostMultiplier;
    public AudioClip clip;

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
        Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);

        player.AddBuff (this);
		z.OnEffectActivated();

		yield return player.WaitUntilLastZoneModified();
		z.OnEffectFinished();
		player.RemoveBuff (this);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		data.moveAxis *= boostMultiplier;
	}
}

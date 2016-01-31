using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectShootStop : ZoneEffect {

    public AudioClip clip;

	public override IEnumerator ApplyEffect (Player player)
	{
        Camera.main.GetComponent<AudioSource>().PlayOneShot(clip);

        foreach (Player p in Game.Instance.Players)
			if(p != player)
				player.AddBuff (this);

		yield return player.WaitUntilLastZoneModified();

		foreach(Player p in Game.Instance.Players)
			if(p != player)
				player.RemoveBuff (this);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		data.fireButtonPressed = false;
	}
}

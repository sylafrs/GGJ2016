using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectFlipCamera : ZoneEffect 
{
	public override IEnumerator ApplyEffect (Player player)
	{
		CameraFlip flipper_le_dauphin = Camera.main.GetComponent<CameraFlip>();
		Zone z = player.LastOwnedZone;

		flipper_le_dauphin.inverse = true;
		yield return player.WaitUntilLastZoneModified();
		flipper_le_dauphin.inverse = (z != player.LastOwnedZone);
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing
	}
}

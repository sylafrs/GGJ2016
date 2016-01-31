using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectVibrationOthers : ZoneEffect {

	public float durationVibration;
	public float cooldownVibration;

	[Range(0, 1)]
	public float leftMotor;

	[Range(0, 1)]
	public float rightMotor;

	public int vibrationsCount;

	private bool mustStop;

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
		mustStop = true;
		yield return null;
		mustStop = false;
		foreach (Player p in Game.Instance.Players)
			if (p != player)
				Game.Instance.StartCoroutine(Vibrate(p));

		z.OnEffectActivated();
		yield return player.WaitUntilLastZoneModified();
		z.OnEffectFinished();

		mustStop = true;
	}

	private IEnumerator Vibrate(Player p)
	{
		int i = 0;

		while (!mustStop && i < vibrationsCount)
		{			
			p.Vibrate(leftMotor, rightMotor);
			yield return Game.Instance.StartCoroutine(Timer(durationVibration));

			p.Vibrate(0, 0);
			yield return Game.Instance.StartCoroutine(Timer(cooldownVibration));

			i++;
		}
	}

	private IEnumerator Timer(float d)
	{
		float t = 0;
		while (!mustStop && t < d)
		{
			yield return null;
			t += Time.deltaTime;
		}
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}
}

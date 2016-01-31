using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class ZoneEffectBump : ZoneEffect {

    public float bumpForce;
    public float duration;

	public override IEnumerator ApplyEffect (Zone z, Player player)
	{
        foreach (Player otherPlayer in Game.Instance.Players)
        {
            if(otherPlayer != player)
            {
                Game.Instance.StartCoroutine(Bump(otherPlayer));
            }
        }

        yield break;
	}

	public override void ApplyInputBuff (ref PlayerInput data)
	{
		// Does nothing.
	}

    IEnumerator Bump(Player player)
    {
        iTween.ShakePosition(player.Position.transform.gameObject, Vector3.up, 1);

        Vector3 bumpVector = Vector3.up * bumpForce;
        player.rigidbody.AddForce(bumpVector, ForceMode.Impulse);

        float baseDrag = player.rigidbody.drag;
        player.rigidbody.drag = 0;
        
        float ratio = 0;
        float t = 0;
        while (t < duration)
        {
            ratio = t / duration;
            player.rigidbody.drag = baseDrag * (1 - ratio);

            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }

        player.rigidbody.drag = baseDrag;
    }

}

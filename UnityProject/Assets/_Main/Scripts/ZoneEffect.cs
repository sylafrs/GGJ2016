using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public abstract class ZoneEffect : ScriptableObject 
{
	public abstract void ApplyEffect(Player player);
	public abstract void ApplyInputBuff(ref PlayerInput data);
}

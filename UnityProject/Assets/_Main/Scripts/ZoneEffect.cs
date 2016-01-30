using UnityEngine;
using System.Collections;

public abstract class ZoneEffect : ScriptableObject 
{
	public abstract IEnumerator ApplyEffect(Player player);
	public abstract void ApplyInputBuff(ref PlayerInput data);
}

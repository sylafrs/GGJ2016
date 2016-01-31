using UnityEngine;
using System.Collections;

public abstract class ZoneEffect : ScriptableObject 
{
	[Range(0, 9)]
	public int NeededLetters;

	public Sprite Picto;

	public abstract IEnumerator ApplyEffect(Player player);
	public abstract void ApplyInputBuff(ref PlayerInput data);
}
